using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.CartServices;

public interface ICartServices
{
    Task<ListDataResultModel<CartResModel>> GetCart(string Token);
    Task<MessageResultModel> AddToCart(CartReqModel cartReqModel , string Token);
    Task<MessageResultModel> UpdateCart(CartReqModel cartReqModel , string Token);
    Task<MessageResultModel> DeleteCart(Guid cartId , string Token);
}