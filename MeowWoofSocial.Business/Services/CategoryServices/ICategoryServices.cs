using MeowWoofSocial.Data.DTO.ResponseModel;

namespace MeowWoofSocial.Business.Services.CategoryServices;

public interface ICategoryServices
{
    Task<ListDataResultModel<CategoryResModel>> GetCategories();
    Task<ListDataResultModel<FilterCategoryResModel>> GetFilterCategories();
    Task<ListDataResultModel<FilterCategoryResModel>> GetFilterCategories(Guid categoryId);
}