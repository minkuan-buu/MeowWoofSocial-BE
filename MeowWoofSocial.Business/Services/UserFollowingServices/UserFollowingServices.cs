using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.UserFollowingRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.UserFollowingServices
{
    public class UserFollowingServices : IUserFollowingServices
    {
        private readonly IUserRepositories _userRepo;
        private readonly IMapper _mapper;
        private readonly IUserFollowingRepositories _userFollowingRepo;

        public UserFollowingServices(IUserFollowingRepositories userFollowingRepo, IUserRepositories userRepo, IMapper mapper)
        {
            _mapper = mapper;
            _userFollowingRepo = userFollowingRepo;
            _userRepo = userRepo;
        }

        public async Task<MessageResultModel> FollowUser(UserFollowingReqModel userFollowing, string token)
        {
            Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
            var userFollow = await _userRepo.GetSingle(x => x.Id == userId);
            var userFollowed = await _userFollowingRepo.GetSingle(x => x.UserId == userFollowing.UserId && x.FollowerId == userId);
            var getUser = await _userRepo.GetSingle(x => x.Id == userFollowing.UserId);
            MessageResultModel result = new();
            try
            {
                if (userFollow == null)
                {
                    result.Message = "User isn't existed!";
                }
                else if (userFollowed != null && userFollowed.Status.Equals(GeneralStatusEnums.Active.ToString()))
                {
                    result.Message = "You have already followed this user";
                }
                else if (getUser == null)
                {
                    result.Message = "The user you are trying to follow does not exist!";
                }
                else
                {
                    var userEntity = _mapper.Map<UserFollowing>(userFollowing);
                    userEntity.FollowerId = userId;
                    userEntity.UserId = userFollowing.UserId;
                    userEntity.Status = GeneralStatusEnums.Active.ToString();
                    await _userFollowingRepo.Insert(userEntity);
                    result.Message = "User followed successfully";
                }
            }
            catch (Exception ex)
            {
                result.Message = $"An error occurred: {ex.Message}";
            }
            return result;
        }
    }
}
