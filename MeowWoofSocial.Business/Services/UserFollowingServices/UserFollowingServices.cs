using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
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

        public async Task<DataResultModel<UserProfilePageResModel>> FollowUser(UserFollowingReqModel userFollowing, string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var getUser = await _userRepo.GetSingle(x => x.Id == userFollowing.UserId, includeProperties: "UserFollowingFollowers");
                MessageResultModel result = new();
                var getUserFollowing = getUser.UserFollowingFollowers.Where(x => x.UserId.Equals(userId) && x.Status.Equals(GeneralStatusEnums.Active.ToString())).FirstOrDefault();
                if (getUserFollowing != null)
                {
                    throw new CustomException("You have already followed this user");
                }
                else if (getUser == null)
                {
                    throw new CustomException("The user you are trying to follow does not exist!");
                }
                else
                {
                    var userEntity = _mapper.Map<UserFollowing>(userFollowing);
                    userEntity.FollowerId = userFollowing.UserId;
                    userEntity.UserId = userId;
                    userEntity.Status = GeneralStatusEnums.Active.ToString();
                    await _userFollowingRepo.Insert(userEntity);
                }
                var followers = await _userFollowingRepo.GetList(x => x.FollowerId.Equals(userFollowing.UserId), includeProperties: "User");
                var followings = await _userFollowingRepo.GetList(x => x.UserId.Equals(userFollowing.UserId), includeProperties: "Follower");
                return new DataResultModel<UserProfilePageResModel>()
                {
                    Data = new UserProfilePageResModel()
                    {
                        Id = getUser.Id,
                        Name = TextConvert.ConvertFromUnicodeEscape(getUser.Name),    
                        Avatar = getUser.Avartar, // Corrected typo
                        CreatedAt = getUser.CreateAt,
                        Email = getUser.Email,
                        IsFollow = followers.Any(x => x.UserId == userId && x.Status == GeneralStatusEnums.Active.ToString()),
                        Follower = followers.Select(x => new UserFollowResModel()
                        {
                            Id = x.User.Id,
                            Avatar = x.User.Avartar, // Corrected typo
                            Name = TextConvert.ConvertFromUnicodeEscape(x.User.Name),
                        }).ToList(),
                        Following = followings.Select(x => new UserFollowResModel()
                        {
                            Id = x.Follower.Id,
                            Avatar = x.Follower.Avartar, // Corrected typo
                            Name = TextConvert.ConvertFromUnicodeEscape(x.Follower.Name),
                        }).ToList()
                    }
                };
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }
    }
}
