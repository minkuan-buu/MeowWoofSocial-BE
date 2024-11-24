using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Repositories.CartRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreProductItemRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;

namespace MeowWoofSocial.Business.Services.CartServices;

public class CartServices : ICartServices
{
    private readonly ICartRepositories _cartRepositories;
    private readonly IPetStoreProductItemRepositories _productItemRepositories;
    private readonly IPetStoreRepositories _storeRepositories;
    private readonly IUserRepositories _userRepositories;
    
    public CartServices(ICartRepositories cartRepositories, IPetStoreProductItemRepositories productItemRepositories, IPetStoreRepositories storeRepositories, IUserRepositories userRepositories)
    {
        _cartRepositories = cartRepositories;
        _productItemRepositories = productItemRepositories;
        _storeRepositories = storeRepositories;
        _userRepositories = userRepositories;
    }
    
    public async Task<List<CartResModel>> GetCart(string Token)
    {
        Guid UserId = new Guid(Authentication.DecodeToken(Token, "userid"));

        var UserCart = await _cartRepositories.GetList(
            x => x.UserId == UserId,
            orderBy: x => x.OrderByDescending(y => y.UpdatedAt ?? y.CreatedAt),
            includeProperties: "ProductItem.Product.PetStore,ProductItem.Product.PetStoreProductAttachments"
        );

        var ReturnUserCart = UserCart
            .GroupBy(x => x.ProductItem.Product.PetStoreId)
            .Select(group =>
            {
                var petStoreName = group.FirstOrDefault()?.ProductItem.Product
                    .PetStore.Name;
                if (petStoreName != null)
                    return new CartResModel()
                    {
                        StoreId = group.Key,
                        StoreName = TextConvert.ConvertFromUnicodeEscape(petStoreName),
                        CartItems = group.Select(cart => new CartDetailResModel()
                        {
                            CartId = cart.Id,
                            ProductItemId = cart.ProductItemId,
                            Attachment = cart.ProductItem.Product.PetStoreProductAttachments.FirstOrDefault()?.Attachment ?? string.Empty,
                            ProductName = TextConvert.ConvertFromUnicodeEscape(cart.ProductItem.Product.Name),
                            ProductItemName = TextConvert.ConvertFromUnicodeEscape(cart.ProductItem.Name),
                            UnitPrice = cart.ProductItem.Price,
                            Quantity = cart.Quantity,
                            Status = cart.ProductItem.Quantity > 0 ? "In Stock" : "Out of Stock"
                        }).ToList()
                    };
                return null;
            }).ToList();
        return ReturnUserCart;
    }
}