using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.PetStoreProductRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;

namespace MeowWoofSocial.Business.Services.PetStoreProductServices;

public class PetStoreProductServices : IPetStoreProductServices
{
    private readonly IMapper _mapper;
    private readonly IPetStoreProductRepositories _petStoreProductRepo;
    private readonly IUserRepositories _userRepo;
    private readonly IPetStoreRepositories _petStoreRepo;
    
    public PetStoreProductServices(IMapper mapper, IPetStoreProductRepositories petStoreProductRepositories, IUserRepositories userRepositories, IPetStoreRepositories petStoreRepositories)
    {
        _mapper = mapper;
        _petStoreProductRepo = petStoreProductRepositories;
        _userRepo = userRepositories;
        _petStoreRepo = petStoreRepositories;
    }
    
    public async Task<DataResultModel<PetStoreProductCreateResModel>> CreatePetStoreProduct(PetStoreProductCreateReqModel petStoreProduct, string token)
    {
        var result = new DataResultModel<PetStoreProductCreateResModel>();
        try
        {
            Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
            var user = await _userRepo.GetSingle(x => x.Id == userId);
            var getpetStore = await _petStoreRepo.GetSingle(x => x.Id == petStoreProduct.PetStoreId, includeProperties: "PetStoreProducts");
            if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
            {
                throw new CustomException("You are banned from creating pet store product due to violate of terms!");
            }

            var newPetStoreProductId = Guid.NewGuid();
            var petStoreProductEntity = _mapper.Map<PetStoreProduct>(petStoreProduct);
            petStoreProductEntity.Id = newPetStoreProductId;
            petStoreProductEntity.CategoryId = petStoreProduct.CategoryId;
            petStoreProductEntity.PetStoreId = petStoreProduct.PetStoreId;
            petStoreProductEntity.Name = TextConvert.ConvertToUnicodeEscape(petStoreProduct.Name);
            petStoreProductEntity.Status = GeneralStatusEnums.Active.ToString();
            petStoreProductEntity.CreateAt = DateTime.Now;

            await _petStoreProductRepo.Insert(petStoreProductEntity);
            result.Data = _mapper.Map<PetStoreProductCreateResModel>(petStoreProductEntity);
        }
        catch (Exception ex)
        {
            throw new CustomException($"An error occurred: {ex.Message}");
        }
        return result;
    }
    
    public async Task<DataResultModel<PetStoreProductUpdateResModel>> UpdatePetStoreProduct(PetStoreProductUpdateReqModel petStoreProduct, string token)
    {
        var result = new DataResultModel<PetStoreProductUpdateResModel>();
        try
        {
            Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
            var user = await _userRepo.GetSingle(x => x.Id == userId);
            var getPetStoreProduct = await _petStoreProductRepo.GetSingle(x => x.Id == petStoreProduct.Id);
            if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
            {
                throw new CustomException("You are banned from updating pet store product due to violate of terms!");
            }
            if (getPetStoreProduct == null)
            {
                throw new CustomException("The pet store product you are trying to update does not exist!");
            }
            getPetStoreProduct.Id = petStoreProduct.Id;
            getPetStoreProduct.Name = TextConvert.ConvertToUnicodeEscape(petStoreProduct.Name);
            getPetStoreProduct.CategoryId = petStoreProduct.CategoryId;
            getPetStoreProduct.UpdateAt = DateTime.Now;

            await _petStoreProductRepo.Update(getPetStoreProduct);
            result.Data = _mapper.Map<PetStoreProductUpdateResModel>(getPetStoreProduct);
        }
        catch (Exception ex)
        {
            throw new CustomException($"An error occurred: {ex.Message}");
        }
        return result;
    }
}