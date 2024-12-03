using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.PetCareCategoryRepositories;

public class PetCareCategoryRepositories : GenericRepositories<PetCareCategory>, IPetCareCategoryRepositories
{
    public PetCareCategoryRepositories(MeowWoofSocialContext context)
        : base(context)
    {
    }
}