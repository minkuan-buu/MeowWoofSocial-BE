﻿using AutoMapper;
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

        public PetStoreServices(IUserRepositories userRepositories, IMapper mapper, IPetStoreRepositories petStoreRepositories)
        {
            _userRepo = userRepositories;
            _petStoreRepositories = petStoreRepositories;
            _mapper = mapper;
        }
        public async Task<DataResultModel<PetStoreCreateResModel>> CreatePetStore(PetStoreCreateReqModel petStore, string token)
        {
            var result = new DataResultModel<PetStoreCreateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from posting due to violate of terms!");
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
        
        public async Task<DataResultModel<PetStoreUpdateResModel>> UpdatePetStore(PetStoreUpdateReqModel petStoreUpdateReq, string token)
        {
            var result = new DataResultModel<PetStoreUpdateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var petStores = await _petStoreRepositories.GetSingle(x => x.Id == petStoreUpdateReq.Id && x.UserId == userId);

                if (petStoreUpdateReq == null)
                {
                    throw new CustomException("PetStore not found or you do not have permission to update this Pet Store");
                }
                
                petStores.Name = TextConvert.ConvertToUnicodeEscape(petStoreUpdateReq.Name ?? string.Empty);
                petStores.Description = TextConvert.ConvertToUnicodeEscape(petStoreUpdateReq.Description ?? string.Empty);
                petStores.Email = petStoreUpdateReq.Email;
                petStores.Phone = petStoreUpdateReq.Phone;
                petStores.Status = GeneralStatusEnums.Active.ToString();
                petStores.UpdateAt = DateTime.Now;

                await _petStoreRepositories.Update(petStores);

                var updatedPetStore = await _petStoreRepositories.GetSingle(x => x.Id == petStoreUpdateReq.Id, includeProperties: "User");

                result.Data = _mapper.Map<PetStoreUpdateResModel>(updatedPetStore);

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }
    }
}
