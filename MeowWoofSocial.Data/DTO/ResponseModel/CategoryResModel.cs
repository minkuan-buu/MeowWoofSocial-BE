namespace MeowWoofSocial.Data.DTO.ResponseModel;

public class CategoryResModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Attachment { get; set; } = null!;
}

public class FilterCategoryResModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Attachment { get; set; } = null!;
    public List<FilterCategoryResModel>? SubCategories { get; set; } = new();
}