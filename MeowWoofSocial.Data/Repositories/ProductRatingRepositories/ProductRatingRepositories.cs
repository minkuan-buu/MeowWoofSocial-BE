using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.ProductRatingRepositories
{
    public class ProductRatingRepositories : GenericRepositories<ProductRating>, IProductRatingRepositories
    {
        public ProductRatingRepositories(MeowWoofSocialContext context)
            : base(context)
        {
        }
    }
}