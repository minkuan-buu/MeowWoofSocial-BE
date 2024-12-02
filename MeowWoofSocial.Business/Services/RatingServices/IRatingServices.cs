using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.RatingServices;

public interface IRatingServices
{
    Task<ListDataResultModel<OrderRatingPetStore>> GetOrderRatingPetStore(string Token, Guid OrderId);
    Task<MessageResultModel> RatingOrder(string Token, List<RatingReqModel> request);
    Task<ListDataResultModel<ProductRatingResModel>> GetProductRating(string Token, Guid ProductId);
}