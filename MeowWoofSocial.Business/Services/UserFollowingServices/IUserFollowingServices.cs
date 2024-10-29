using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.UserFollowingServices
{
    public interface IUserFollowingServices
    {
        Task<DataResultModel<UserProfilePageResModel>> FollowUser(UserFollowingReqModel userFollowing, string token);
    }
}
