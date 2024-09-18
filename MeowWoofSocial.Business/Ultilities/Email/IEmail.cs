using MeowWoofSocial.Data.DTO.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Ultilities.Email
{
    public interface IEmail
    {
        Task SendEmail(string Subject, List<EmailReqModel> emailReqModels);
        //Task<bool> SendListEmail(string Subject, List<EmailSendingModel> sendingList);
    }
}
