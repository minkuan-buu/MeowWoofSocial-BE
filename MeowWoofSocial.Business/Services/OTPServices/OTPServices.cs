using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Business.Ultilities.Email;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.IOTPRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.OTPServices
{
    public class OTPServices : IOTPServices
    {
        private readonly IUserRepositories _UserRepositories;
        private readonly IOTPRepositories _OTPRepositories;
        private readonly IEmail _email;

        public OTPServices(IUserRepositories UserRepo, IOTPRepositories OTPRepo, IEmail email)
        {
            _UserRepositories = UserRepo;
            _OTPRepositories = OTPRepo;
            _email = email;
        }

        private string CreateOTPCode()
        {
            Random rnd = new();
            return rnd.Next(100000, 999999).ToString();
        }

        public async Task<MessageResultModel> SendOTPEmailRequest(string Email)
        {
            try
            {
                var User = await _UserRepositories.GetSingle(x => x.Email.Equals(Email), includeProperties: "Otps");
                if (User == null)
                {
                    throw new CustomException("User not found!");
                }
                var getActiveOtp = User.Otps.Where(x => x.Status.Equals(GeneralStatusEnums.Active.ToString())).ToList();
                foreach (var otp in getActiveOtp)
                {
                    if ((otp.ExpiredDate - DateTime.Now).TotalMinutes > 8)
                    {
                        throw new CustomException("Can not send OTP right now!");
                    }
                    else
                    {
                        otp.Status = GeneralStatusEnums.Inactive.ToString();
                    }
                }
                await _OTPRepositories.UpdateRange(getActiveOtp);
                string OTPCode = CreateOTPCode();
                string FilePath = "./ResetPassword.html";
                string Html = File.ReadAllText(FilePath);
                for (int i = 0; i < OTPCode.Length; i++)
                {
                    Html = Html.Replace($"[{i}]", OTPCode[i].ToString());
                }
                List<EmailReqModel> ListEmailReq = new()
                {
                    new EmailReqModel { Email = Email, HtmlContent = Html },
                };
                Otp Otp = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = User.Id,
                    Code = OTPCode,
                    ExpiredDate = DateTime.Now.AddMinutes(10),
                    Status = GeneralStatusEnums.Active.ToString(),
                    IsUsed = false
                };
                await _OTPRepositories.Insert(Otp);
                await _email.SendEmail("Đặt lại mật khẩu", ListEmailReq);
                return new MessageResultModel
                {
                    Message = "Ok"
                };
            }
            catch (Exception e)
            {
                throw new CustomException("Error" + e.Message);
            }
        }

        public async Task<DataResultModel<UserTemp>> VerifyOTPCode(string Email, string OTPCode)
        {
            try
            {
                var User = await _UserRepositories.GetSingle(x => x.Email.Equals(Email), includeProperties: "Otps");
                if (User == null)
                {
                    throw new CustomException("User not found");
                }
                var GetOTP = User.Otps.FirstOrDefault(x => x.Code.Equals(OTPCode) && x.Status.Equals(GeneralStatusEnums.Active.ToString()));
                if (GetOTP != null)
                {
                    if ((DateTime.Now - GetOTP.ExpiredDate).TotalMinutes > 10 || GetOTP.IsUsed)
                    {
                        throw new CustomException("The OTP is expired!");
                    }
                    GetOTP.IsUsed = true;
                    GetOTP.Status = GeneralStatusEnums.Inactive.ToString();
                    await _OTPRepositories.Update(GetOTP);
                    User.Status = AccountStatusEnums.ResetPassword.ToString();
                    await _UserRepositories.Update(User);
                    var Result = new UserTemp()
                    {
                        TempToken = Authentication.GenerateTempJWT(Email)
                    };
                    return new DataResultModel<UserTemp>
                    {
                        Data = Result
                    };
                }
                else
                {
                    throw new CustomException("The OTP is invalid!");
                }
            }
            catch (Exception e)
            {
                throw new CustomException("Error" + e.Message);
            }
        }
    }
}
