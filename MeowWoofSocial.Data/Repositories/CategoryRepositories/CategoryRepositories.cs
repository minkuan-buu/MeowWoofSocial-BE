using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.CategoryRepositories;

public class CategoryRepositories : GenericRepositories<Category>, ICategoryRepositories
{
    public CategoryRepositories(MeowWoofSocialContext context)
        : base(context)
    {
    }
}