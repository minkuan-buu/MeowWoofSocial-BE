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
using MeowWoofSocial.Data.Repositories.TransactionRepositories;
using MeowWoofSocial.Data.Repositories.UserAddressRepositories;
using AutoMapper;
using MeowWoofSocial.Data.Repositories;
using MeowWoofSocial.Data.Repositories.CartRepositories;
using MeowWoofSocial.Data.Repositories.PetCareBookingRepositories;
using Net.payOS;
using Net.payOS.Types;
using ItemData = Net.payOS.Types.ItemData;
using PaymentData = Net.payOS.Types.PaymentData;
using Transaction = MeowWoofSocial.Data.Entities.Transaction;

namespace MeowWoofSocial.Business.Services.TransactionServices
{
    public class TransactionServices : ITransactionServices
    {
        private readonly IHubContext<TransactionHub> _transactionHub;
        private readonly ILogger<TransactionServices> _logger;
        private readonly IOrderRepositories _orderRepositories;
        private readonly IMapper _mapper;
        private readonly IUserAddressRepositories _userAddressRepositories;
        private readonly IOrderDetailRepositories _orderDetailRepositories;
        private readonly ITransactionRepositories _transactionRepositories;
        private readonly IPetStoreProductItemRepositories _petStoreProductItemRepositories;
        private readonly ICartRepositories _cartRepositories;
        private readonly IPetCareBookingRepositories _petCareBookingRepositories;
        private PayOS _payOS;
        private const string MEMO_PREFIX = "DH"; // Prefix for order ID

        public TransactionServices(IHubContext<TransactionHub> transactionHub, ILogger<TransactionServices> logger, IOrderRepositories orderRepositories, IPetStoreProductItemRepositories petStoreProductItemRepositories, IOrderDetailRepositories orderDetailRepositories, IUserAddressRepositories userAddressRepositories, ITransactionRepositories transactionRepositories, IMapper mapper, ICartRepositories cartRepositories, IPetCareBookingRepositories petCareBookingRepositories)
        {
            _payOS = new PayOS("421fdf87-bbe1-4694-a76c-17627d705a85", "7a2f58da-4003-4349-9e4b-f6bbfc556c9b", "da759facf68f863e0ed11385d3bf9cf24f35e2b171d1fa8bae8d91ce1db9ff0c");
            _mapper = mapper;
            _transactionRepositories = transactionRepositories;
            _petCareBookingRepositories = petCareBookingRepositories;
            _userAddressRepositories = userAddressRepositories;
            _orderDetailRepositories = orderDetailRepositories;
            _petStoreProductItemRepositories = petStoreProductItemRepositories;
            _orderRepositories = orderRepositories;
            _transactionHub = transactionHub;
            _cartRepositories = cartRepositories;
            _logger = logger;
        }

        // public async Task<MessageResultModel> HandleTransactions(TransactionResponseDto transactions)
        // {
        //     foreach (var transaction in transactions.Data)
        //     {
        //         var description = transaction.Description.ToString();
        //         var orderId = ParseOrderId(description);
        //
        //         if (orderId == null)
        //         {
        //             _logger.LogWarning($"Unable to identify order_id from the payment description: {description}");
        //             continue;
        //         } else
        //         {
        //             _logger.LogInformation($"Identified order_id: {orderId}");
        //
        //             var Order = await _orderRepositories.GetSingle(x => x.RefId.Equals(orderId.Value.ToString()) && x.Status.Equals(OrderEnums.Pending.ToString()), includeProperties:"Transactions");
        //             if(Order == null)
        //             {
        //                 _logger.LogWarning($"{orderId} is invalid");
        //                 continue;
        //             }
        //
        //             var paidAmount = (decimal)transaction.Amount;
        //             var totalFormatted = paidAmount.ToString("N0");
        //             var orderNote = $"Casso thông báo nhận <b>{totalFormatted}</b> VND, nội dung <b>{description}</b> chuyển vào <b>STK {transaction.BankSubAccId}</b>";
        //             if (paidAmount == Order.Price)
        //             {
        //                 TransactionResModel transactionResModel = new()
        //                 {
        //                     Id = Order.Id,
        //                     RefId = orderId.Value,
        //                     StatusPayment = TransactionEnums.Success.ToString(),
        //                     TotalPrice = Order.Price
        //                 };
        //                 
        //                 var TransactionPending = Order.Transactions.FirstOrDefault(x => x.Status.Equals(TransactionEnums.Pending.ToString()));
        //                 TransactionPending.Status = TransactionEnums.Success.ToString();
        //                 TransactionPending.CassoTransactionId = transaction.Id.ToString();
        //                 TransactionPending.CassoRefId = transaction.Tid;
        //                 TransactionPending.FinishTransactionAt = transaction.When;
        //                 
        //                 Order.Status = OrderEnums.Delivering.ToString();
        //                 
        //                 await _orderRepositories.Update(Order);
        //                 await _transactionRepositories.Update(TransactionPending);
        //                 await NotifyClients(orderId.Value, transactionResModel);
        //             }
        //         }
        //     }
        //     return new MessageResultModel()
        //     {
        //         Message = "Ok"
        //     };
        // }

        public async Task<String> CreatePaymentUrl(string Token, Guid Id)
        {
            var userId = new Guid(Authentication.DecodeToken(Token, "userid"));
            var order = await _orderRepositories.GetSingle(x => x.Id.Equals(Id) && x.UserId.Equals(userId) && x.Status.Equals(OrderEnums.Pending.ToString()), includeProperties: "Transactions,OrderDetails.ProductItem.Product");
            
            // Kiểm tra xem có giao dịch PENDING hay không
            var pendingTransaction = order.Transactions.Any(x => x.Status.Equals(TransactionEnums.PENDING.ToString()));

            if (pendingTransaction) // Chỉ cần kiểm tra nếu có giao dịch đang chờ xử lý
            {
                // Lấy PaymentLinkId của giao dịch đang chờ xử lý
                var transaction = order.Transactions.FirstOrDefault(x => x.Status.Equals(TransactionEnums.PENDING.ToString()));
                if (transaction != null)
                {
                    return $"https://pay.payos.vn/web/{transaction.PaymentLinkId}";
                }
                else
                {
                    // Trường hợp không có giao dịch PENDING thì trả về null hoặc thông báo lỗi
                    return "No pending transaction found.";
                }
            }
            else
            {
                // Nếu không có giao dịch PENDING thì tạo link thanh toán mới
                var OrderPaymentRefId = int.Parse(GenerateRandomRefId());
                List<ItemData> itemDatas = order.OrderDetails
                    .Select(x => new ItemData(
                        $"{TextConvert.ConvertFromUnicodeEscape(x.ProductItem.Product.Name)} ({TextConvert.ConvertFromUnicodeEscape(x.ProductItem.Name)})",
                        x.Quantity,
                        (int)x.UnitPrice
                    ))
                    .ToList();

                PaymentData paymentData = new PaymentData(OrderPaymentRefId, (int)order.Price, "",
                    itemDatas, cancelUrl: "https://meowwoofsocial.com/payment", returnUrl: "https://meowwoofsocial.com/payment");

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
                Transaction NewTransaction = new Transaction()
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    OrderPaymentRefId = OrderPaymentRefId,
                    Status = TransactionEnums.PENDING.ToString(),
                    PaymentLinkId = createPayment.paymentLinkId,
                    CreatedAt = DateTime.Now,
                };
                await _transactionRepositories.Insert(NewTransaction);
                return createPayment.checkoutUrl;
            }
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
                
                var UserCart = await _cartRepositories.GetList(x => x.UserId.Equals(userId));
                foreach (var CartItem in UserCart)
                {
                    CartItem.OrderId = null;
                }

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

                        if (detail.CartId != null)
                        {
                            var cart = UserCart.FirstOrDefault(x => x.Id.Equals(detail.CartId));
                            if (cart != null)
                            {
                                cart.OrderId = existingOrder.Id;
                                await _cartRepositories.Update(cart);
                            }
                        }
                    }

                    existingOrder.Price = existingOrder.OrderDetails
                        .Sum(od => od.UnitPrice * od.Quantity);
                    existingOrder.UserAddressId = userAddress != null ? userAddress.Id : null;
                    existingOrder.UpdatedAt = DateTime.Now;

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
                        CreatedAt = DateTime.Now,
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
                    
                    foreach (var detail in orderDetails)
                    {
                        if (detail.CartId != null)
                        {
                            var cart = UserCart.FirstOrDefault(x => x.Id.Equals(detail.CartId));
                            if (cart != null)
                            {
                                cart.OrderId = newOrder.Id;
                                await _cartRepositories.Update(cart);
                            }
                        }
                    }
                
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
        
        public async Task<DataResultModel<OrderResModel>> GetOrder(Guid id, string token)
        {
            var userId = new Guid(Authentication.DecodeToken(token, "userid"));
            
            // Lấy đơn hàng từ repository với các thuộc tính liên quan
            var order = await _orderRepositories.GetSingle(o => o.Id == id && o.UserId == userId && o.Status.Equals(OrderEnums.Pending.ToString()), 
                includeProperties: "OrderDetails.ProductItem.Product.PetStoreProductAttachments,OrderDetails.ProductItem.Product.PetStore,UserAddress");

            if (order == null)
            {
                throw new CustomException("Order not found");
            }

            // Tạo OrderResModel và ánh xạ các thuộc tính
            var orderResModel = new OrderResModel
            {
                Id = order.Id,
                TotalPrice = order.Price,
                UserAddress = order.UserAddress != null ? new OrderUserAddress
                {
                    Id = order.UserAddress.Id,
                    Name = TextConvert.ConvertFromUnicodeEscape(order.UserAddress.Name),
                    Phone = order.UserAddress.Phone,
                    Address = TextConvert.ConvertFromUnicodeEscape(order.UserAddress.Address),
                    IsDefault = order.UserAddress.Status.Equals(UserAddressEnums.Default.ToString())
                } : null,
                PetStores = order.OrderDetails
                    .GroupBy(detail => detail.ProductItem.Product.PetStore.Id) // Nhóm theo PetStore Id
                    .Select(group => new OrderPetStore
                    {
                        Id = group.Key,
                        Name = TextConvert.ConvertFromUnicodeEscape(group.First().ProductItem.Product.PetStore.Name),
                        Phone = group.First().ProductItem.Product.PetStore.Phone,
                        OrderDetails = group.Select(detail => new OrderDetailResModel
                        {
                            Id = detail.Id,
                            Attachment = detail.ProductItem.Product.PetStoreProductAttachments.FirstOrDefault()?.Attachment ?? string.Empty,
                            ProductName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Product.Name),
                            ProductItemName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Name),
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice
                        }).ToList()
                    }).ToList()
            };

            return new DataResultModel<OrderResModel> { Data = orderResModel };
        }

        public async Task<DataResultModel<UserAddressCreateResModel>> ChangeOrderAddress(Guid orderId, Guid addressId, string token)
        {
            try
            {
                var userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var order = await _orderRepositories.GetSingle(x => x.Id.Equals(orderId) && x.UserId.Equals(userId) && x.Status.Equals(OrderEnums.Pending.ToString()));
                
                var newAddress = await _userAddressRepositories.GetSingle(x => x.UserId.Equals(userId) && x.Id.Equals(addressId));

                if (order == null) {
                    throw new CustomException("Order not found!");
                }

                if(newAddress == null)
                {
                    throw new CustomException("Address not found!");
                }

                order.UpdatedAt = DateTime.Now;
                order.UserAddressId = newAddress.Id;
                await _orderRepositories.Update(order);
                return new DataResultModel<UserAddressCreateResModel>()
                {
                    Data = _mapper.Map<UserAddressCreateResModel>(newAddress)
                };
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }

        public async Task<DataResultModel<OrderPaymentResModel>> HandleCheckTransaction(string id, string token)
        {
            try
            {
                var userId = new Guid(Authentication.DecodeToken(token, "userid"));
        
                var transaction = await _transactionRepositories.GetSingle(x => x.PaymentLinkId.Equals(id) && x.Status.Equals(TransactionEnums.PENDING.ToString()), includeProperties: "Order.OrderDetails.ProductItem.Product.PetStoreProductAttachments,Order.PetCareBookings.PetStore,Order.PetCareBookings.PetCareBookingDetails.Pet,Order.OrderDetails.ProductItem.Product.PetStore,Order.UserAddress");

                if (transaction.Order == null)
                {
                    throw new CustomException("Order not found");
                }
        
                if (transaction == null)
                {
                    throw new CustomException("Transaction not found");
                }

                var orderResModel = new OrderPaymentResModel();
                if (transaction.Order.PetCareBookings.FirstOrDefault(x =>
                        x.UserId == userId && x.Status.Equals(OrderEnums.Pending.ToString())) != null)
                {
                    orderResModel = new OrderPaymentResModel()
                    {
                        Id = transaction.Order.Id,
                        TotalPrice = transaction.Order.Price,
                        StatusPayment = transaction.Status,
                        UserAddress = transaction.Order.UserAddress != null ? new OrderUserAddress
                        {
                            Id = transaction.Order.UserAddress.Id,
                            Name = TextConvert.ConvertFromUnicodeEscape(transaction.Order.UserAddress.Name),
                            Phone = transaction.Order.UserAddress.Phone,
                            Address = TextConvert.ConvertFromUnicodeEscape(transaction.Order.UserAddress.Address),
                            IsDefault = transaction.Order.UserAddress.Status.Equals(UserAddressEnums.Default.ToString())
                        } : null,
                        PetStores = transaction.Order.PetCareBookings.Select(x => new OrderPetStore()
                        {
                            Id = x.PetStore.Id,
                            Name = TextConvert.ConvertFromUnicodeEscape(x.PetStore.Name),
                            Phone = x.PetStore.Phone,
                            OrderDetails = x.PetCareBookingDetails.Select(detail => new OrderDetailResModel
                            {
                                Id = detail.Id,
                                Attachment = "https://cdn-icons-png.flaticon.com/512/3028/3028549.png",
                                ProductName = $"Booking {TextConvert.ConvertFromUnicodeEscape(x.PetStore.Name)}",
                                ProductItemName = TextConvert.ConvertFromUnicodeEscape(detail.Pet.Name),
                                Quantity = 1,
                                UnitPrice = 200000
                            }).ToList()
                        }).ToList()
                    };
                }
                else
                {
                    orderResModel = new OrderPaymentResModel()
                    {
                        Id = transaction.Order.Id,
                        TotalPrice = transaction.Order.Price,
                        StatusPayment = transaction.Status,
                        UserAddress = transaction.Order.UserAddress != null ? new OrderUserAddress
                        {
                            Id = transaction.Order.UserAddress.Id,
                            Name = TextConvert.ConvertFromUnicodeEscape(transaction.Order.UserAddress.Name),
                            Phone = transaction.Order.UserAddress.Phone,
                            Address = TextConvert.ConvertFromUnicodeEscape(transaction.Order.UserAddress.Address),
                            IsDefault = transaction.Order.UserAddress.Status.Equals(UserAddressEnums.Default.ToString())
                        } : null,
                        PetStores = transaction.Order.OrderDetails
                            .GroupBy(detail => detail.ProductItem.Product.PetStore.Id) // Nhóm theo PetStore Id
                            .Select(group => new OrderPetStore
                            {
                                Id = group.Key,
                                Name = TextConvert.ConvertFromUnicodeEscape(group.First().ProductItem.Product.PetStore.Name),
                                Phone = group.First().ProductItem.Product.PetStore.Phone,
                                OrderDetails = group.Select(detail => new OrderDetailResModel
                                {
                                    Id = detail.Id,
                                    Attachment = detail.ProductItem.Product.PetStoreProductAttachments.FirstOrDefault()?.Attachment ?? string.Empty,
                                    ProductName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Product.Name),
                                    ProductItemName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Name),
                                    Quantity = detail.Quantity,
                                    UnitPrice = detail.UnitPrice
                                }).ToList()
                            }).ToList()
                    };
                }
                
                
                PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(transaction.OrderPaymentRefId);

                if (paymentLinkInformation == null)
                {
                    throw new CustomException("Transaction not found");
                }

                if (paymentLinkInformation.status.Equals(TransactionEnums.PAID.ToString()))
                {
                    if (transaction.Order.PetCareBookings.FirstOrDefault(x => x.UserId == userId && x.Status.Equals(OrderEnums.Pending.ToString())) != null)
                    {
                        var petCareBooking = transaction.Order.PetCareBookings.FirstOrDefault(x => x.UserId == userId && x.Status.Equals(OrderEnums.Pending.ToString()));
                        petCareBooking.Status = OrderEnums.Success.ToString();
                        await _petCareBookingRepositories.Update(petCareBooking);
                    }
                    var getTransaction = paymentLinkInformation.transactions.FirstOrDefault();
                    transaction.TransactionReference = getTransaction.reference;
                    transaction.FinishedTransactionAt = DateTime.Parse(getTransaction.transactionDateTime);
                    orderResModel.StatusPayment = TransactionEnums.PAID.ToString();
                    transaction.Status = TransactionEnums.PAID.ToString();
                    transaction.Order.Status = OrderEnums.Delivering.ToString();
                    transaction.Order.UpdatedAt = DateTime.Now;
                    orderResModel.StatusPayment = TransactionEnums.PAID.ToString();
                    foreach (var orderDetail in transaction.Order.OrderDetails)
                    {
                        orderDetail.ProductItem.Quantity = orderDetail.ProductItem.Quantity - orderDetail.Quantity;
                    }
                    var UserCart = await _cartRepositories.GetList(x => x.UserId.Equals(userId) && x.OrderId == transaction.Order.Id);
                    List<Cart> CartItems = new List<Cart>();
                    foreach (var CartItem in UserCart)
                    {
                        CartItems.Add(CartItem);
                    }
                    await _cartRepositories.DeleteRange(CartItems);
                } else if (paymentLinkInformation.status.Equals(TransactionEnums.CANCELLED.ToString()))
                {
                    if (transaction.Order.PetCareBookings.FirstOrDefault(x => x.UserId == userId && x.Status.Equals(OrderEnums.Pending.ToString())) != null)
                    {
                        var petCareBooking = transaction.Order.PetCareBookings.FirstOrDefault(x => x.UserId == userId && x.Status.Equals(OrderEnums.Pending.ToString()));
                        petCareBooking.Status = OrderEnums.Cancelled.ToString();
                        await _petCareBookingRepositories.Update(petCareBooking);
                    }
                    transaction.Status = TransactionEnums.CANCELLED.ToString();
                    transaction.Order.UpdatedAt = DateTime.Now;
                    transaction.Order.Status = OrderEnums.Cancelled.ToString();
                    orderResModel.StatusPayment = TransactionEnums.CANCELLED.ToString();
                }

                await _transactionRepositories.Update(transaction);

                return new DataResultModel<OrderPaymentResModel> { Data = orderResModel };
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }

        public async Task<ListDataResultModel<ListOrderResModel>> GetOrderList(string token)
        {
            var userId = new Guid(Authentication.DecodeToken(token, "userid"));
            
            // Lấy đơn hàng từ repository với các thuộc tính liên quan
            var orders = await _orderRepositories.GetList(o => o.UserId == userId && !o.Status.Equals(OrderEnums.Pending.ToString()), 
                includeProperties: "OrderDetails.ProductItem.Product.PetStoreProductAttachments,OrderDetails.ProductItem.Product.PetStore,UserAddress");

            if (orders == null)
            {
                throw new CustomException("Order not found");
            }

            // Tạo OrderResModel và ánh xạ các thuộc tính
            var orderResModel = orders.Select(order => new ListOrderResModel
            {
                Id = order.Id,
                TotalPrice = order.Price,
                Status = order.Status,
                UserAddress = order.UserAddress != null ? new OrderUserAddress
                {
                    Id = order.UserAddress.Id,
                    Name = TextConvert.ConvertFromUnicodeEscape(order.UserAddress.Name),
                    Phone = order.UserAddress.Phone,
                    Address = TextConvert.ConvertFromUnicodeEscape(order.UserAddress.Address),
                    IsDefault = order.UserAddress.Status.Equals(UserAddressEnums.Default.ToString())
                } : null,
                PetStores = order.OrderDetails
                    .GroupBy(detail => detail.ProductItem.Product.PetStore.Id) // Nhóm theo PetStore Id
                    .Select(group => new OrderPetStore
                    {
                        Id = group.Key,
                        Name = TextConvert.ConvertFromUnicodeEscape(group.First().ProductItem.Product.PetStore.Name),
                        Phone = group.First().ProductItem.Product.PetStore.Phone,
                        OrderDetails = group.Select(detail => new OrderDetailResModel
                        {
                            Id = detail.Id,
                            Attachment = detail.ProductItem.Product.PetStoreProductAttachments.FirstOrDefault()?.Attachment ?? string.Empty,
                            ProductName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Product.Name),
                            ProductItemName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Name),
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice
                        }).ToList()
                    }).ToList()
            }).ToList();

            return new ListDataResultModel<ListOrderResModel>() { Data = orderResModel };
        }

        public async Task<DataResultModel<ListOrderResModel>> GetTrackingOrder(Guid id, string token)
        {
            var userId = new Guid(Authentication.DecodeToken(token, "userid"));
            
            // Lấy đơn hàng từ repository với các thuộc tính liên quan
            var order = await _orderRepositories.GetSingle(o => o.Id == id && o.UserId == userId && !o.Status.Equals(OrderEnums.Pending.ToString()), 
                includeProperties: "OrderDetails.ProductItem.Product.PetStoreProductAttachments,OrderDetails.ProductItem.Product.PetStore,UserAddress");

            if (order == null)
            {
                throw new CustomException("Order not found");
            }

            // Tạo OrderResModel và ánh xạ các thuộc tính
            var orderResModel = new ListOrderResModel
            {
                Id = order.Id,
                TotalPrice = order.Price,
                Status = order.Status,
                UserAddress = order.UserAddress != null ? new OrderUserAddress
                {
                    Id = order.UserAddress.Id,
                    Name = TextConvert.ConvertFromUnicodeEscape(order.UserAddress.Name),
                    Phone = order.UserAddress.Phone,
                    Address = TextConvert.ConvertFromUnicodeEscape(order.UserAddress.Address),
                    IsDefault = order.UserAddress.Status.Equals(UserAddressEnums.Default.ToString())
                } : null,
                PetStores = order.OrderDetails
                    .GroupBy(detail => detail.ProductItem.Product.PetStore.Id) // Nhóm theo PetStore Id
                    .Select(group => new OrderPetStore
                    {
                        Id = group.Key,
                        Name = TextConvert.ConvertFromUnicodeEscape(group.First().ProductItem.Product.PetStore.Name),
                        Phone = group.First().ProductItem.Product.PetStore.Phone,
                        OrderDetails = group.Select(detail => new OrderDetailResModel
                        {
                            Id = detail.Id,
                            Attachment = detail.ProductItem.Product.PetStoreProductAttachments.FirstOrDefault()?.Attachment ?? string.Empty,
                            ProductName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Product.Name),
                            ProductItemName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Name),
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice
                        }).ToList()
                    }).ToList()
            };

            return new DataResultModel<ListOrderResModel> { Data = orderResModel };
        }
    }
}
