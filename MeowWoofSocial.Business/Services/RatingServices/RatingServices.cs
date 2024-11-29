using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Repositories.OrderRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreProductRatingRepositories;

namespace MeowWoofSocial.Business.Services.RatingServices;

public class RatingServices : IRatingServices
{
    private readonly IPetStoreProductRatingRepositories _ratingRepositories;
    private readonly IOrderRepositories _orderRepositories;
    
    public RatingServices(IPetStoreProductRatingRepositories ratingRepositories, IOrderRepositories orderRepositories)
    {
        _ratingRepositories = ratingRepositories;
        _orderRepositories = orderRepositories;
    }

    public async Task<ListDataResultModel<OrderRatingPetStore>> GetOrderRatingPetStore(string Token, Guid OrderId)
    {
        var userId = new Guid(Authentication.DecodeToken(Token, "userid"));

        var OrderItem = await _orderRepositories.GetSingle(
            x => x.Id.Equals(OrderId) && x.UserId.Equals(userId),
            includeProperties: "OrderDetails.ProductItem.PetStoreProductRatings,OrderDetails.ProductItem.Product.PetStore,OrderDetails.ProductItem.Product.PetStoreProductAttachments"
        );

        if (OrderItem == null)
        {
            throw new CustomException("Order not found");
        }

        var ExistingRating = OrderItem.OrderDetails
            .SelectMany(detail => detail.ProductItem.PetStoreProductRatings)
            .Where(rating => rating.UserId.Equals(userId))
            .ToDictionary(rating => rating.ProductItemId, rating => rating.Rating);

        var OrderRatingPetStore = OrderItem.OrderDetails
            .Where(detail => !ExistingRating.ContainsKey(detail.ProductItemId)) // Lọc sản phẩm chưa được đánh giá
            .GroupBy(detail => detail.ProductItem.Product.PetStore.Id) // Nhóm theo PetStore Id
            .Select(group => new OrderRatingPetStore
            {
                Id = group.Key,
                Name = TextConvert.ConvertFromUnicodeEscape(group.First().ProductItem.Product.PetStore.Name),
                Phone = group.First().ProductItem.Product.PetStore.Phone,
                OrderDetails = group.Select(detail => new OrderRatingDetailResModel
                {
                    Id = detail.Id,
                    Attachment = detail.ProductItem.Product.PetStoreProductAttachments.FirstOrDefault()?.Attachment ?? string.Empty,
                    ProductName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Product.Name),
                    ProductItemName = TextConvert.ConvertFromUnicodeEscape(detail.ProductItem.Name),
                }).ToList()
            }).ToList();

        return new ListDataResultModel<OrderRatingPetStore>()
        {
            Data = OrderRatingPetStore
        };

    }
}