using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.PetStoreProductRepositories;

public class PetStoreProductRepositories : GenericRepositories<PetStoreProduct>, IPetStoreProductRepositories
{
    public PetStoreProductRepositories(MeowWoofSocialContext context)
        : base(context)
    {
    }
}