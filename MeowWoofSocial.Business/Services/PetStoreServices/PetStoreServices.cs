using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Business.Services.CloudServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.PetStoreRepositories;
using MeowWoofSocial.Data.Repositories.UserFollowingRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.PetStoreServices
{
    public class PetStoreServices : IPetStoreServices
    {
        private readonly IMapper _mapper;
        private readonly IUserRepositories _userRepo;
        private readonly IPetStoreRepositories _petStoreRepositories;

        public PetStoreServices(IUserRepositories userRepositories, IMapper mapper,
            IPetStoreRepositories petStoreRepositories)
        {
            _userRepo = userRepositories;
            _petStoreRepositories = petStoreRepositories;
            _mapper = mapper;
        }

        public async Task<DataResultModel<PetStoreCreateResModel>> CreatePetStore(PetStoreCreateReqModel petStore,
            string token)
        {
            var result = new DataResultModel<PetStoreCreateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from creating pet store due to violate of terms!");
                }

                var existingPetStore = await _petStoreRepositories.GetSingle(x => x.UserId == userId);
                if (existingPetStore != null)
                {
                    throw new CustomException("You already have a pet store.");
                }

                var newPetStoreId = Guid.NewGuid();
                var petStoreEntity = _mapper.Map<PetStore>(petStore);
                petStoreEntity.Id = newPetStoreId;
                petStoreEntity.UserId = userId;
                petStoreEntity.Name = TextConvert.ConvertToUnicodeEscape(petStore.Name);
                petStoreEntity.Description = TextConvert.ConvertToUnicodeEscape(petStore.Description);
                petStoreEntity.Email = petStore.Email;
                petStoreEntity.Phone = petStore.Phone;
                petStoreEntity.CreateAt = DateTime.Now;
                petStoreEntity.Status = GeneralStatusEnums.Active.ToString();

                await _petStoreRepositories.Insert(petStoreEntity);
                result.Data = _mapper.Map<PetStoreCreateResModel>(petStoreEntity);
            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }

            return result;
        }

        public async Task<DataResultModel<PetStoreUpdateResModel>> UpdatePetStore(
            PetStoreUpdateReqModel petStoreUpdateReq, string token)
        {
            var result = new DataResultModel<PetStoreUpdateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var petStores =
                    await _petStoreRepositories.GetSingle(x => x.Id == petStoreUpdateReq.Id && x.UserId == userId);

                if (petStoreUpdateReq == null)
                {
                    throw new CustomException(
                        "PetStore not found or you do not have permission to update this Pet Store");
                }

                petStores.Name = TextConvert.ConvertToUnicodeEscape(petStoreUpdateReq.Name ?? string.Empty);
                petStores.Description =
                    TextConvert.ConvertToUnicodeEscape(petStoreUpdateReq.Description ?? string.Empty);
                petStores.Email = petStoreUpdateReq.Email;
                petStores.Phone = petStoreUpdateReq.Phone;
                petStores.Status = GeneralStatusEnums.Active.ToString();
                petStores.UpdateAt = DateTime.Now;

                await _petStoreRepositories.Update(petStores);

                var updatedPetStore =
                    await _petStoreRepositories.GetSingle(x => x.Id == petStoreUpdateReq.Id, includeProperties: "User");

                result.Data = _mapper.Map<PetStoreUpdateResModel>(updatedPetStore);

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }

            return result;
        }

        public async Task<DataResultModel<PetStoreDeleteResModel>> DeletePetStore(
            PetStoreDeleteReqModel PetStoreDeleteReq, string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var petStore = await _petStoreRepositories.GetSingle(c => c.Id == PetStoreDeleteReq.PetStoreId);

                if (petStore == null || petStore.UserId != userId)
                {
                    throw new CustomException("PetStore not found or does not belong to the user.");
                }

                await _petStoreRepositories.Delete(petStore);

                var result = _mapper.Map<PetStoreDeleteResModel>(petStore);
                return new DataResultModel<PetStoreDeleteResModel> { Data = result };
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error deleting PetStore: {ex.Message}");
            }
        }

        public async Task<DataResultModel<PetStoreCreateResModel>> GetPetStoreByID(Guid petStoreId, string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));

                var petStoreEntity = await _petStoreRepositories.GetSingle(
                    x => x.Id == petStoreId,
                    includeProperties: "User"
                );

                if (petStoreEntity == null)
                {
                    throw new CustomException("Pet Store not found");
                }

                var petStoreDetail = _mapper.Map<PetStoreCreateResModel>(petStoreEntity);

                return new DataResultModel<PetStoreCreateResModel>
                {
                    Data = petStoreDetail
                };
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error fetching Pet Store by ID: {ex.Message}");
            }
        }

        public async Task<ListDataResultModel<PetStoreServiceResModel>> GetPetStoreService(string Type)
        {
            var petStoreServices = await _petStoreRepositories.GetList(
                x => x.TypeStore == Type && x.Status == GeneralStatusEnums.Active.ToString(),
                includeProperties: "PetStoreRatings");
            var result = petStoreServices.Select(x => new PetStoreServiceResModel()
            {
                Id = x.Id,
                Name = TextConvert.ConvertFromUnicodeEscape(x.Name),
                Description = TextConvert.ConvertFromUnicodeEscape(x.Description),
                Attachment = x.Attachment ?? string.Empty,
                Type = x.TypeStore,
                AverageRating = x.PetStoreRatings.Count == 0 ? 0 : x.PetStoreRatings.Sum(y => y.Rating) / x.PetStoreRatings.Count
            }).ToList();
            return new ListDataResultModel<PetStoreServiceResModel> { Data = result };
        }
    }
}

