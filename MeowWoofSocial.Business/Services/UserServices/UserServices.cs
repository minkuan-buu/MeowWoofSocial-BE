using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;

namespace MeowWoofSocial.Business.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepositories _userRepositories;
        private readonly IMapper _mapper;

        public UserServices(IUserRepositories userRepositories, IMapper mapper)
        {
            _userRepositories = userRepositories;
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
    }
}
