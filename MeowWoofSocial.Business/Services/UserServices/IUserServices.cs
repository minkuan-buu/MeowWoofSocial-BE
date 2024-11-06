using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.UserServices
{
    public interface IUserServices
    {
        Task<DataResultModel<UserLoginResModel>> LoginUser(UserLoginReqModel User);
        Task<MessageResultModel> RegisterUser(UserRegisterReqModel newUser);
        Task<DataResultModel<UserProfilePageResModel>> GetUserById(Guid userId, string token);
        Task<DataResultModel<UpdateUserProfileResModel>> UpdateUserProfile(UpdateUserProfileReqModel profileUpdateReq, string token);
        Task<DataResultModel<UpdateUserAvartarResModel>> UpdateUserAvartar(UpdateUserAvartarReqModel profileUpdateReq, string token);
    }
}
