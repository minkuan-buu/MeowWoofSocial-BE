using MeowWoofSocial.Data.DTO.RequestModel;
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
    }
}
