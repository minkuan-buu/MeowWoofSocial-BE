﻿using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.TransactionServices
{
    public interface ITransactionServices
    {
        public Task<MessageResultModel> HandleTransactions(TransactionResponseDto transactions);
        public Task<DataResultModel<OrderCreateResModel>> CreateOrder(List<OrderDetailCreateReqModel> request, string token);
        public Task<DataResultModel<OrderResModel>> GetOrder(Guid id, string token);
        public Task<DataResultModel<TransactionPendingResModel>> CheckRefId(int refId, string token);
        public Task<DataResultModel<UserAddressCreateResModel>> ChangeOrderAddress(Guid orderId, Guid addressId,  string token);
        public Task<MessageResultModel> CancelTransaction(Guid id, string token);
    }
}
