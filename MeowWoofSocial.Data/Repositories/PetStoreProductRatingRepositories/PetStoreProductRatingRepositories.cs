using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.PetStoreProductRatingRepositories;

public class PetStoreProductRatingRepositories : GenericRepositories<PetStoreProductRating>, IPetStoreProductRatingRepositories
{
    public PetStoreProductRatingRepositories(MeowWoofSocialContext context)
        : base(context)
    {
    }
}