using MeowWoofSocial.Data.DTO.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.OTPServices
{
    public interface IOTPServices
    {
        Task<MessageResultModel> SendOTPEmailRequest(string Email);
        Task<DataResultModel<UserTemp>> VerifyOTPCode(string Email, string OTPCode);
    }
}
