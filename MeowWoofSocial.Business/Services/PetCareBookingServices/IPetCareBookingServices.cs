using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.PetCareBookingServices
{
    public interface IPetCareBookingServices
    {
        Task<DataResultModel<PetCareBookingCreateResModel>> CreatePetCareBooking(PetCareBookingCreateReqModel petCareBooking,
            string token);
        Task<MessageResultModel> CancelPetCareBooking(Guid PetCareBookingId, string Token);

    }
}