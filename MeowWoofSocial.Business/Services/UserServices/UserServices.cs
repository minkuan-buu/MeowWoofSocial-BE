using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.UserFollowingRepositories;

namespace MeowWoofSocial.Business.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepositories _userRepositories;
        private readonly IUserFollowingRepositories _userFollowingRepositories;
        private readonly IMapper _mapper;

        public UserServices(IUserRepositories userRepositories, IUserFollowingRepositories userFollowingRepositories, IMapper mapper)
        {
            _userRepositories = userRepositories;
            _userFollowingRepositories = userFollowingRepositories;
            _mapper = mapper;
        }
        
        public async Task<DataResultModel<UserLoginResModel>>  LoginUser(UserLoginReqModel User)
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
            if(user == null)
            {
                throw new CustomException("User not found");
            }
            var UserResModel = new UserProfilePageResModel()
            {
                Id = userId,
                Name = TextConvert.ConvertFromUnicodeEscape(user.Name),
                Avartar = user.Avartar,
                CreatedAt = user.CreateAt,
                Email = user.Email,
            };
            var followers = await _userFollowingRepositories.GetList(x => x.FollowerId.Equals(userId), includeProperties: "User.UserFollowingFollowers");
            var followings = await _userFollowingRepositories.GetList(x => x.UserId.Equals(userId), includeProperties: "Follower.UserFollowingFollowers");
            List<UserFollowResModel> ListFollower = new();
            List<UserFollowResModel> ListFollowing = new();
            foreach(var follower in followers)
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
    }
}
