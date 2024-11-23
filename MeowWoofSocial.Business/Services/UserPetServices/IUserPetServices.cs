using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.UserPetServices;

public interface IUserPetServices
{
    Task<DataResultModel<UserPetCreateResMdoel>> CreateUserPet(UserPetCreateReqMdoel uerPetReq, string token);
}