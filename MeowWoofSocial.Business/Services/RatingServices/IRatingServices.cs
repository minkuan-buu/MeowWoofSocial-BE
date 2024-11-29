using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.RatingServices;

public interface IRatingServices
{
    Task<ListDataResultModel<OrderRatingPetStore>> GetOrderRatingPetStore(string Token, Guid OrderId);
}