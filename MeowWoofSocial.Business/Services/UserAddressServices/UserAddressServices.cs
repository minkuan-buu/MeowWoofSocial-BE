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

        public async Task<DataResultModel<UserAddressUpdateResModel>> UpdateUserAddress(UserAddressUpdateReqModel userAddressReq, string token)
        {
            var result = new DataResultModel<UserAddressUpdateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var userAddress = await _userAddressRepo.GetSingle(x => x.Id == userAddressReq.Id && x.UserId == userId);

                if (userAddressReq == null)
                {
                    throw new CustomException("Address not found or you do not have permission to update this Address");
                }

                userAddress.Name = TextConvert.ConvertToUnicodeEscape(userAddressReq.Name ?? string.Empty);
                userAddress.Phone = userAddressReq.Phone;
                userAddress.Address = TextConvert.ConvertToUnicodeEscape(userAddressReq.Address ?? string.Empty);
                userAddress.Status = UserAddressEnums.Active.ToString();
                userAddress.UpdateAt = DateTime.Now;

                await _userAddressRepo.Update(userAddress);

                var updatedUserAddress = await _userAddressRepo.GetSingle(x => x.Id == userAddressReq.Id, includeProperties: "User");

                result.Data = _mapper.Map<UserAddressUpdateResModel>(updatedUserAddress);

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }

        public async Task<DataResultModel<UserAddressSetDefaultResModel>> SetDefaultUserAddress(UserAddressSetDefaultReqModel userAddressReq, string token)
        {
            var result = new DataResultModel<UserAddressSetDefaultResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var userAddress = await _userAddressRepo.GetSingle(x => x.Id == userAddressReq.Id && x.UserId == userId);

                if (userAddressReq == null)
                {
                    throw new CustomException("Address not found or you do not have permission to update this Address");
                }

                userAddress.Status = UserAddressEnums.Default.ToString();
                userAddress.UpdateAt = DateTime.Now;

                await _userAddressRepo.Update(userAddress);

                var updatedUserAddress = await _userAddressRepo.GetSingle(x => x.Id == userAddressReq.Id, includeProperties: "User");

                result.Data = _mapper.Map<UserAddressSetDefaultResModel>(updatedUserAddress);

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }

        public async Task<MessageResultModel> DeleteUserAddress(UserAddressDeleteReqModel userAddressDeleteReq, string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var userAddess = await _userAddressRepo.GetSingle(p => p.Id == userAddressDeleteReq.UserAddressId);
                if (userAddess == null)
                {
                    throw new CustomException("User Address not found.");
                }
                if (userAddess.UserId != userId)
                {
                    throw new CustomException("User Address item is not belong to user.");
                }

                await _userAddressRepo.Delete(userAddess);

                var result = _mapper.Map<UserAddressDeleteResModel>(userAddess);
                return new MessageResultModel
                {
                    Message = "Ok"
                };
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error removing address: {ex.Message}");
            }
        }

        public async Task<ListDataResultModel<UserAddressCreateResModel>> GetUserAddress(Guid userId)
        {
            try
            {
                List<UserAddressCreateResModel> userAddress = new();

                var allUserAddrees = await _userAddressRepo.GetList(x => x.UserId.Equals(userId));

                userAddress = allUserAddrees
                    .Select(x => new UserAddressCreateResModel
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Name = x.Name,
                        Phone = x.Phone,
                        Address = x.Address
                    })
                    .ToList();

                return new ListDataResultModel<UserAddressCreateResModel>
                {
                    Data = userAddress
                };
            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred while fetching User Address: {ex.Message}");
            }
        }
    }
}
