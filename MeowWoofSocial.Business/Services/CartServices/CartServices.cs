using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
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

    public async Task<MessageResultModel> AddToCart(CartReqModel cartReqModel, string Token)
    {
        Guid UserId = new Guid(Authentication.DecodeToken(Token, "userid"));
        var checkExist = await _cartRepositories.GetSingle(x => x.ProductItemId.Equals(cartReqModel.ProductItemId) && x.UserId.Equals(UserId));
        if (checkExist != null)
        {
            checkExist.Quantity += cartReqModel.Quantity;
            checkExist.UpdatedAt = DateTime.Now;
            await _cartRepositories.Update(checkExist);
        }
        else
        {
            var productItem = await _productItemRepositories.GetSingle(x => x.Id.Equals(cartReqModel.ProductItemId));
            if (productItem == null)
                throw new CustomException("Product Item not found");
            if (productItem.Quantity < cartReqModel.Quantity)
                throw new CustomException("Product Item out of stock");
            var cart = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                ProductItemId = cartReqModel.ProductItemId,
                Quantity = cartReqModel.Quantity,
                CreatedAt = DateTime.Now
            };
            await _cartRepositories.Insert(cart);
        }

        return new MessageResultModel()
        {
            Message = "Ok"
        };
    }

    public async Task<MessageResultModel> UpdateCart(CartReqModel cartReqModel, string Token)
    {
        throw new NotImplementedException();
    }
}