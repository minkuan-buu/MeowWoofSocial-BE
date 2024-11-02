using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.PetStoreProductItemRepositories;

public class PetStoreProductItemRepositories : GenericRepositories<PetStoreProductItem>, IPetStoreProductItemRepositories
{
    public PetStoreProductItemRepositories(MeowWoofSocialContext context)
        : base(context)
    {
    }
}