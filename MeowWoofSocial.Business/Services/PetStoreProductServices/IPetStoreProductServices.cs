﻿using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.PetStoreProductServices;

public interface IPetStoreProductServices
{
    Task<DataResultModel<PetStoreProductCreateResModel>> CreatePetStoreProduct(PetStoreProductCreateReqModel petStoreProduct,
        string token);
    Task<DataResultModel<PetStoreProductUpdateResModel>> UpdatePetStoreProduct(PetStoreProductUpdateReqModel petStoreProduct, string token);
    Task<DataResultModel<PetStoreProductDeleteResModel>> DeletePetStoreProduct(PetStoreProductDeleteReqModel PetStoreDeleteReq, string token);
    Task<ListDataResultModel<GetAllPetStoreProductResModel>> GetAllPetStoreProduct(PetStoreProductReq petStoreProductReq);
}