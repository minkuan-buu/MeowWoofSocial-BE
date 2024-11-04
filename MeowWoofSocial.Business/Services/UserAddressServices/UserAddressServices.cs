using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.UserAddressRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.UserAddressServices
{
    public class UserAddressServices : IUserAddressServices
    {
        private readonly IUserAddressRepositories _userAddressRepo;
        private readonly IMapper _mapper;
        private readonly IUserRepositories _userRepo;

        public UserAddressServices(IUserAddressRepositories userAddressRepo, IMapper mapper, IUserRepositories userRepo)
        {
            _userAddressRepo = userAddressRepo;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        public async Task<DataResultModel<UserAddressCreateResModel>> CreateUserAddress(UserAddressCreateReqModel userAddressReq, string token)
        {
            var result = new DataResultModel<UserAddressCreateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from creating address due to violate of terms!");
                }

                var newUserAddressId = Guid.NewGuid();
                var userAddressEntity = _mapper.Map<UserAddress>(userAddressReq);
                userAddressEntity.Id = newUserAddressId;
                userAddressEntity.UserId = userId;
                userAddressEntity.Name = TextConvert.ConvertToUnicodeEscape(userAddressReq.Name);
                userAddressEntity.Phone = userAddressReq.Phone;
                userAddressEntity.Address = TextConvert.ConvertToUnicodeEscape(userAddressReq.Address);
                userAddressEntity.CreateAt = DateTime.Now;
                userAddressEntity.Status = UserAddressEnums.Active.ToString();

                await _userAddressRepo.Insert(userAddressEntity);
                result.Data = _mapper.Map<UserAddressCreateResModel>(userAddressEntity);
            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }
    }
}
