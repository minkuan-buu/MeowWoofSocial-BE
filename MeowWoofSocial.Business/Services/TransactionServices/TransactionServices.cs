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

namespace MeowWoofSocial.Business.Services.TransactionServices
{
    public class TransactionServices : ITransactionServices
    {
        private readonly IHubContext<TransactionHub> _transactionHub; 
        private readonly ILogger<TransactionServices> _logger;
        private readonly IOrderRepositories _orderRepositories;
        private const string MEMO_PREFIX = "DH"; // Prefix for order ID

        public TransactionServices(IHubContext<TransactionHub> transactionHub, ILogger<TransactionServices> logger, IOrderRepositories orderRepositories)
        {
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
    }
}
