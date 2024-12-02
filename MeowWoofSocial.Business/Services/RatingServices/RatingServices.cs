using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
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
                    ProductItemId = detail.ProductItemId,
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

    public async Task<MessageResultModel> RatingOrder(string Token, List<RatingReqModel> request)
    {
        var userId = new Guid(Authentication.DecodeToken(Token, "userid"));

        var checkExist = await _ratingRepositories.GetList(x => x.UserId.Equals(userId));

        var productsNotRated = request.Where(x => !checkExist.Select(r => r.ProductItemId).Contains(x.ProductItemId)).ToList();

        if (!productsNotRated.Any())
        {
            return new MessageResultModel
            {
                Message = "Rating completed successfully"
            };
        }

        foreach (var product in productsNotRated)
        {
            await _ratingRepositories.Insert(new PetStoreProductRating()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductItemId = product.ProductItemId,
                Rating = product.StarRating,
                Comment = TextConvert.ConvertToUnicodeEscape(product.Comment ?? string.Empty),
                CreatedAt = DateTime.Now
            });
        }

        return new MessageResultModel
        {
            Message = "Rating completed successfully"
        };
    }
    public async Task<ListDataResultModel<ProductRatingResModel>> GetProductRating(string Token, Guid ProductId)
    {
        var userId = new Guid(Authentication.DecodeToken(Token, "userid"));

        var ProductRatings = await _ratingRepositories.GetList(
            x => x.ProductItem.ProductId.Equals(ProductId),
            includeProperties: "User,ProductItem"
        );

        var ProductRatingResModel = ProductRatings.Select(rating => new ProductRatingResModel
        {
            Id = rating.Id,
            Author = new AuthorRatingResModel
            {
                Id = rating.UserId,
                Name = TextConvert.ConvertFromUnicodeEscape(rating.User.Name),
                Attachment = rating.User.Avartar ?? string.Empty
            },
            ProductItem = new ProductItemRatingResModel
            {
                ProductItemId = rating.ProductItemId,
                ProductItemName = TextConvert.ConvertFromUnicodeEscape(rating.ProductItem.Name)
            },
            Rating = rating.Rating,
            Comment = TextConvert.ConvertFromUnicodeEscape(rating.Comment ?? String.Empty),
            CreatedAt = rating.CreatedAt
        }).ToList();

        return new ListDataResultModel<ProductRatingResModel>()
        {
            Data = ProductRatingResModel
        };
    }
}