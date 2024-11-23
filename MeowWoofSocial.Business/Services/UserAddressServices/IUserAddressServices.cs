using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.UserAddressServices
{
    public interface IUserAddressServices
    {
        Task<DataResultModel<UserAddressCreateResModel>> CreateUserAddress(UserAddressCreateReqModel userAddressReq, string token);
        Task<DataResultModel<UserAddressUpdateResModel>> UpdateUserAddress(Guid id, UserAddressUpdateReqModel userAddressReq, string token);
        Task<DataResultModel<UserAddressSetDefaultResModel>> SetDefaultUserAddress(Guid id, string token);
        Task<MessageResultModel> DeleteUserAddress(Guid id, string token);
        Task<ListDataResultModel<UserAddressCreateResModel>> GetUserAddress(string token);
    }
}
