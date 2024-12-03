using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.UserPetServices;

public interface IUserPetServices
{
    Task<DataResultModel<UserPetCreateResMdoel>> CreateUserPet(UserPetCreateReqMdoel uerPetReq, string token);
    Task<DataResultModel<UserPetUpdateResMdoel>> UpdateUserPet(UserPetUpdateReqMdoel userPetUpdateReq,
        string token);
    Task<MessageResultModel> DeleteUserPet(Guid PetId , string Token);
    Task<ListDataResultModel<UserPetModel>> GetUserPetByUserID(string token);
}