using System.CodeDom.Compiler;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.CategoryRepositories;

namespace MeowWoofSocial.Business.Services.CategoryServices;

public class CategoryServices : ICategoryServices
{
    private readonly ICategoryRepositories _categoryRepositories;
    
    public CategoryServices(ICategoryRepositories categoryRepositories)
    {
        _categoryRepositories = categoryRepositories;
    }
    
    public async Task<ListDataResultModel<CategoryResModel>> GetCategories()
    {
        var categories = await _categoryRepositories.GetList(x =>
            !x.ParentCategoryId.HasValue && x.Status.Equals(GeneralStatusEnums.Active.ToString()));
        var ReturnResult = categories.Select(x => new CategoryResModel()
        {
            Id = x.Id,
            Name = TextConvert.ConvertFromUnicodeEscape(x.Name),
            Attachment = x.Attachment
        }).ToList();
        
        return new ListDataResultModel<CategoryResModel>()
        {
            Data = ReturnResult
        };
    }

    public async Task<ListDataResultModel<FilterCategoryResModel>> GetFilterCategories()
    {
        var categories = await _categoryRepositories.GetList(x =>
            !x.ParentCategoryId.HasValue && x.Status.Equals(GeneralStatusEnums.Active.ToString()));

        var result = new List<FilterCategoryResModel>();

        foreach (var category in categories)
        {
            result.Add(new FilterCategoryResModel
            {
                Id = category.Id,
                Name = TextConvert.ConvertFromUnicodeEscape(category.Name),
                Attachment = category.Attachment,
                SubCategories = await GetSubCategoriesAsync(category.Id)
            });
        }

        return new ListDataResultModel<FilterCategoryResModel>
        {
            Data = result
        };
    }

    private async Task<List<FilterCategoryResModel>> GetSubCategoriesAsync(Guid parentId)
    {
        var subCategories = await _categoryRepositories.GetList(x =>
            x.ParentCategoryId == parentId && x.Status.Equals(GeneralStatusEnums.Active.ToString()));

        return subCategories.Select(sub => new FilterCategoryResModel
        {
            Id = sub.Id,
            Name = TextConvert.ConvertFromUnicodeEscape(sub.Name),
            Attachment = sub.Attachment,
            SubCategories = new() // Optionally fetch nested subcategories if needed
        }).ToList();
    }

    public async Task<ListDataResultModel<FilterCategoryResModel>> GetFilterCategories(Guid categoryId)
    {
        // Lấy category cha từ categoryId
        var parentCategory = await _categoryRepositories.GetSingle(x =>
            x.Id == categoryId && x.Status.Equals(GeneralStatusEnums.Active.ToString()));

        // Lấy các subCategories của category cha
        var subCategories = await _categoryRepositories.GetList(x =>
            x.ParentCategoryId == categoryId && x.Status.Equals(GeneralStatusEnums.Active.ToString()));

        var returnResult = new List<FilterCategoryResModel>();

        // Nếu có category cha, thêm category cha vào kết quả trả về
        if (parentCategory != null)
        {
            var parentCategoryModel = new FilterCategoryResModel
            {
                Id = parentCategory.Id,
                Name = TextConvert.ConvertFromUnicodeEscape(parentCategory.Name),
                Attachment = parentCategory.Attachment,
                SubCategories = subCategories.Select(x => new FilterCategoryResModel
                {
                    Id = x.Id,
                    Name = TextConvert.ConvertFromUnicodeEscape(x.Name),
                    Attachment = x.Attachment
                }).ToList() // Gán danh sách subCategories vào thuộc tính SubCategories của category cha
            };

            returnResult.Add(parentCategoryModel);
        }

        return new ListDataResultModel<FilterCategoryResModel>
        {
            Data = returnResult
        };
    }

}