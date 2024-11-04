using Google.Apis.Logging;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.OrderRepositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.PetStoreProductItemRepositories;
using MeowWoofSocial.Data.Repositories.UserAddressRepositories;

namespace MeowWoofSocial.Business.Services.TransactionServices
{
    public class TransactionServices : ITransactionServices
    {
        private readonly IHubContext<TransactionHub> _transactionHub;
        private readonly ILogger<TransactionServices> _logger;
        private readonly IOrderRepositories _orderRepositories;
        private readonly IUserAddressRepositories _userAddressRepositories;
        private readonly IOrderDetailRepositories _orderDetailRepositories;
        private readonly IPetStoreProductItemRepositories _petStoreProductItemRepositories;
        private const string MEMO_PREFIX = "DH"; // Prefix for order ID

        public TransactionServices(IHubContext<TransactionHub> transactionHub, ILogger<TransactionServices> logger, IOrderRepositories orderRepositories, IPetStoreProductItemRepositories petStoreProductItemRepositories, IOrderDetailRepositories orderDetailRepositories, IUserAddressRepositories userAddressRepositories)
        {
            _userAddressRepositories = userAddressRepositories;
            _orderDetailRepositories = orderDetailRepositories;
            _petStoreProductItemRepositories = petStoreProductItemRepositories;
            _orderRepositories = orderRepositories;
            _transactionHub = transactionHub;
            _logger = logger;
        }

        public async Task<MessageResultModel> HandleTransactions(TransactionResponseDto transactions)
        {
            foreach (var transaction in transactions.Data)
            {
                var description = transaction.Description.ToString();
                var orderId = ParseOrderId(description);

                if (orderId == null)
                {
                    _logger.LogWarning($"Unable to identify order_id from the payment description: {description}");
                    continue;
                } else
                {
                    _logger.LogInformation($"Identified order_id: {orderId}");

                    var Order = await _orderRepositories.GetSingle(x => x.RefId.Equals(orderId.Value.ToString()) && x.Status.Equals(OrderEnums.Pending.ToString()));
                    if(Order == null)
                    {
                        _logger.LogWarning($"{orderId} is invalid");
                        continue;
                    }

                    var paidAmount = (decimal)transaction.Amount;
                    var totalFormatted = paidAmount.ToString("N0");
                    var orderNote = $"Casso thông báo nhận <b>{totalFormatted}</b> VND, nội dung <b>{description}</b> chuyển vào <b>STK {transaction.BankSubAccId}</b>";
                    if (paidAmount == Order.Price)
                    {
                        TransactionResModel transactionResModel = new()
                        {
                            Id = Order.Id,
                            RefId = orderId.Value,
                            StatusPayment = TransactionEnums.Success.ToString(),
                            TotalPrice = Order.Price
                        };

                        Order.Status = OrderEnums.Success.ToString();
                        await _orderRepositories.Update(Order);
                        await NotifyClients(orderId.Value, transactionResModel);
                    }
                }
            }
            return new MessageResultModel()
            {
                Message = "Ok"
            };
        }

        private async Task NotifyClients(int orderId, TransactionResModel Data)
        {
            await _transactionHub.Clients.Group(orderId.ToString()).SendAsync("ReceiveTransactionUpdate", new { orderId, Data });
        }

        private int? ParseOrderId(string description)
        {
            var regex = new Regex($@"{MEMO_PREFIX}\d+", RegexOptions.IgnoreCase);
            var match = regex.Match(description);

            if (!match.Success)
                return null;

            var orderCode = match.Value;
            var orderIdString = orderCode.Substring(MEMO_PREFIX.Length);
            return int.TryParse(orderIdString, out var orderId) ? orderId : (int?)null;
        }
        
        public async Task<DataResultModel<OrderCreateResModel>> CreateOrder(List<OrderDetailCreateReqModel> orderDetails, string token)
        {
            try
            {
                var userId = new Guid(Authentication.DecodeToken(token, "userid"));
                
                var userAddress = await _userAddressRepositories.GetSingle(u => u.UserId == userId && u.Status.Equals(UserAddressEnums.Default.ToString()));

                var productItemIds = orderDetails.Select(d => d.ProductItemId).ToList();

                var productItems = await _petStoreProductItemRepositories.GetList(p => productItemIds.Contains(p.Id));

                foreach (var detail in orderDetails)
                {
                    var productItem = productItems.FirstOrDefault(p => p.Id == detail.ProductItemId);
                    if (productItem == null || productItem.Quantity < detail.Quantity)
                    {
                        throw new CustomException($"Product item {detail.ProductItemId} does not have enough stock.");
                    }
                }

                var existingOrder = await _orderRepositories.GetSingle(
                    o => o.UserId == userId && o.Status == OrderEnums.Pending.ToString(),
                    includeProperties: "OrderDetails");

                if (existingOrder != null)
                {
                    var newProductItemIds = orderDetails.Select(d => d.ProductItemId).ToHashSet();

                    var itemsToRemove = existingOrder.OrderDetails
                        .Where(od => !newProductItemIds.Contains(od.ProductItemId))
                        .ToList();

                    await _orderDetailRepositories.DeleteRange(itemsToRemove); 

                    foreach (var detail in orderDetails)
                    {
                        var existingOrderDetail = existingOrder.OrderDetails
                            .FirstOrDefault(od => od.ProductItemId == detail.ProductItemId);

                        if (existingOrderDetail != null)
                        {
                            existingOrderDetail.Quantity = detail.Quantity;
                            existingOrderDetail.UnitPrice = detail.UnitPrice;
                            existingOrderDetail.UpdateAt = DateTime.Now;
                            await _orderDetailRepositories.Update(existingOrderDetail);
                        }
                        else
                        {
                            var newOrderDetail = new OrderDetail
                            {
                                Id = Guid.NewGuid(),
                                OrderId = existingOrder.Id,
                                ProductItemId = detail.ProductItemId,
                                UnitPrice = detail.UnitPrice,
                                Quantity = detail.Quantity,
                                Status = OrderEnums.Pending.ToString(),
                                CreateAt = DateTime.Now
                            };
                            await _orderDetailRepositories.Insert(newOrderDetail);
                        }
                    }

                    existingOrder.Price = existingOrder.OrderDetails
                        .Sum(od => od.UnitPrice * od.Quantity);
                    existingOrder.UserAddressId = userAddress != null ? userAddress.Id : null;
                    existingOrder.UpdateAt = DateTime.Now;

                    await _orderRepositories.Update(existingOrder); // Cập nhật đơn hàng

                    return new DataResultModel<OrderCreateResModel>()
                    {
                        Data = new OrderCreateResModel()
                        {
                            Id = existingOrder.Id,
                        }
                    };
                }
                else
                {
                    var newOrder = new Order
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Price = orderDetails.Sum(d => d.UnitPrice * d.Quantity),
                        Status = OrderEnums.Pending.ToString(),
                        CreateAt = DateTime.Now,
                        RefId = GenerateRandomRefId(),
                        UserAddressId = userAddress != null ? userAddress.Id : null,
                        OrderDetails = orderDetails.Select(d => new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            ProductItemId = d.ProductItemId,
                            UnitPrice = d.UnitPrice,
                            Quantity = d.Quantity,
                            Status = OrderEnums.Pending.ToString(),
                            CreateAt = DateTime.Now
                        }).ToList()
                    };

                    await _orderRepositories.Insert(newOrder);
                    return new DataResultModel<OrderCreateResModel>()
                    {
                        Data = new OrderCreateResModel()
                        {
                            Id = newOrder.Id,
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }
        
        private string GenerateRandomRefId()
        {
            var random = new Random();
            return random.Next(10000000, 99999999).ToString();
        }
    }
}
