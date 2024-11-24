using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.CartServices;

public interface ICartServices
{
    Task<List<CartResModel>> GetCart(string Token);
}