﻿using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.UserFollowingRepositories;
using MeowWoofSocial.Business.Services.CloudServices;
using System.Text.RegularExpressions;
using System.Text;

namespace MeowWoofSocial.Business.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepositories _userRepositories;
        private readonly IUserFollowingRepositories _userFollowingRepositories;
        private readonly IMapper _mapper;
        private readonly ICloudStorage _cloudStorage;

        public UserServices(IUserRepositories userRepositories, IUserFollowingRepositories userFollowingRepositories, IMapper mapper, ICloudStorage cloudStorage)
        {
            _userRepositories = userRepositories;
            _userFollowingRepositories = userFollowingRepositories;
            _mapper = mapper;
            _cloudStorage = cloudStorage;
        }

        public async Task<DataResultModel<UserLoginResModel>> LoginUser(UserLoginReqModel User)
        {
            var user = await _userRepositories.GetSingle(x => x.Email.Equals(User.Email));
            if (user == null)
            {
                throw new CustomException("Account Not Found!");
            }
            var checkPassword = Authentication.VerifyPasswordHashed(User.Password, user.Salt, user.Password);
            if (!checkPassword)
            {
                throw new CustomException("Wrong Password!");
            }
            if (user.Status.Equals(AccountStatusEnums.ResetPassword.ToString()))
            {
                user.Status = AccountStatusEnums.Active.ToString();
                await _userRepositories.Update(user);
            }
            UserLoginResModel ResUser = _mapper.Map<UserLoginResModel>(user);
            ResUser.Token = Authentication.GenerateJWT(user);
            return new DataResultModel<UserLoginResModel>()
            {
                Data = ResUser
            };
        }

        public async Task<MessageResultModel> RegisterUser(UserRegisterReqModel newUserReq)
        {
            var user = await _userRepositories.GetSingle(x => x.Email.Equals(newUserReq.Email));
            if (user != null)
            {
                throw new CustomException("Email Is Existed!");
            }
            User newUser = _mapper.Map<User>(newUserReq);
            var SaltAndHasedPassword = Authentication.CreateHashPassword(newUserReq.Password);
            newUser.Role = RoleEnums.User.ToString();
            newUser.Salt = SaltAndHasedPassword.Salt;
            newUser.Password = SaltAndHasedPassword.HashedPassword;
            newUser.Status = GeneralStatusEnums.Active.ToString();
            await _userRepositories.Insert(newUser);
            return new MessageResultModel()
            {
                Message = "Ok"
            };
        }

        public async Task<DataResultModel<UserProfilePageResModel>> GetUserById(Guid userId, string token)
        {
            Guid userViewId = new Guid(Authentication.DecodeToken(token, "userid"));
            var user = await _userRepositories.GetSingle(x => x.Id.Equals(userId));
            if (user == null)
            {
                throw new CustomException("User not found");
            }
            var UserResModel = new UserProfilePageResModel()
            {
                Id = userId,
                Name = TextConvert.ConvertFromUnicodeEscape(user.Name),
                Avatar = user.Avartar,
                CreatedAt = user.CreateAt,
                Email = user.Email,
            };
            var followers = await _userFollowingRepositories.GetList(x => x.FollowerId.Equals(userId), includeProperties: "User.UserFollowingFollowers");
            var followings = await _userFollowingRepositories.GetList(x => x.UserId.Equals(userId), includeProperties: "Follower.UserFollowingFollowers");
            List<UserFollowResModel> ListFollower = new();
            List<UserFollowResModel> ListFollowing = new();
            foreach (var follower in followers)
            {
                var FollowerModel = new UserFollowResModel()
                {
                    Id = follower.User.Id,
                    Name = TextConvert.ConvertFromUnicodeEscape(follower.User.Name),
                    Avatar = follower.User.Avartar,
                    IsFollow = follower.User.UserFollowingFollowers.Any(x => x.UserId.Equals(userViewId)),
                };
                ListFollower.Add(FollowerModel);
            }

            foreach (var following in followings)
            {
                var FollowingModel = new UserFollowResModel()
                {
                    Id = following.Follower.Id,
                    Name = TextConvert.ConvertFromUnicodeEscape(following.Follower.Name),
                    Avatar = following.Follower.Avartar,
                    IsFollow = following.Follower.UserFollowingFollowers.Any(x => x.UserId.Equals(userViewId)),
                };
                ListFollowing.Add(FollowingModel);
            }
            UserResModel.IsFollow = followers.Any(x => x.UserId.Equals(userViewId));
            UserResModel.Follower = ListFollower;
            UserResModel.Following = ListFollowing;

            return new DataResultModel<UserProfilePageResModel>
            {
                Data = UserResModel,
            };
        }

        public async Task<DataResultModel<UpdateUserProfileResModel>> UpdateUserProfile(UpdateUserProfileReqModel profileUpdateReq, string token)
        {
            var result = new DataResultModel<UpdateUserProfileResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var userProfile = await _userRepositories.GetSingle(x => x.Id == profileUpdateReq.Id && x.Id == userId);

                if (userProfile == null || userProfile.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from update profile due to violate of terms!");
                }
                userProfile.Email = profileUpdateReq.Email;
                userProfile.Name = TextConvert.ConvertToUnicodeEscape(profileUpdateReq.Name ?? string.Empty);
                userProfile.Phone = profileUpdateReq.Phone;
                userProfile.UpdateAt = DateTime.Now;
                
                await _userRepositories.Update(userProfile);
                result.Data = _mapper.Map<UpdateUserProfileResModel>(userProfile);

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }

            return result;
        }

        public async Task<DataResultModel<UpdateUserAvartarResModel>> UpdateUserAvartar(UpdateUserAvartarReqModel profileUpdateReq, string token)
        {
            var result = new DataResultModel<UpdateUserAvartarResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var userProfile = await _userRepositories.GetSingle(x => x.Id.Equals(userId));

                if (userProfile == null || userProfile.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from update profile due to violate of terms!");
                }                
                userProfile.UpdateAt = DateTime.Now;

                string filePath = $"user/{userId}/avatar";
                if (profileUpdateReq.Avartar != null)
                {
                    var avartar = await _cloudStorage.UploadSingleFile(profileUpdateReq.Avartar, filePath);
                    userProfile.Avartar = avartar;
                }
                await _userRepositories.Update(userProfile);
                result.Data = _mapper.Map<UpdateUserAvartarResModel>(userProfile);

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }

            return result;
        }

        public async Task<MessageResultModel> ResetPassword(UserResetPasswordReqModel ReqModel, string token)
        {
            try
            {
                var email = Authentication.DecodeToken(token, "email");
                var user = await _userRepositories.GetSingle(x => x.Email.Equals(email));
                if (user == null)
                {
                    throw new CustomException("User not found!");
                }
                if (ReqModel.NewPassword != ReqModel.ConfirmPassword)
                {
                    throw new CustomException("New password and confirm password is not match!");
                }
                if (ReqModel.NewPassword.Length < 6)
                {
                    throw new CustomException("Password must be at least 6 characters!");
                }
                var Auth = Authentication.CreateHashPassword(ReqModel.NewPassword);
                user.Password = Auth.HashedPassword;
                user.Salt = Auth.Salt;
                user.Status = GeneralStatusEnums.Active.ToString();
                await _userRepositories.Update(user);
                return new MessageResultModel
                {
                    Message = "Ok"
                };
            }
            catch (Exception ex)
            {
                throw new CustomException("Error: " + ex.Message);
            }
        }
    }
}
