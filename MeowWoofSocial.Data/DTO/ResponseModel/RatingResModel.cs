namespace MeowWoofSocial.Data.DTO.ResponseModel;

public class RatingResModel
{
    
}

public class OrderRatingPetStore
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public List<OrderRatingDetailResModel> OrderDetails { get; set; } = new();
}
    
public class OrderRatingDetailResModel
{
    public Guid ProductItemId { get; set; }
    public string Attachment { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductItemName { get; set; } = null!;
}

public class ProductRatingResModel
{
    public Guid Id { get; set; }
    public AuthorRatingResModel Author { get; set; } = new();
    public ProductItemRatingResModel ProductItem { get; set; } = new();
    public decimal Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuthorRatingResModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Attachment { get; set; } = null!;
}

public class ProductItemRatingResModel
{
    public Guid ProductItemId { get; set; }
    public string ProductItemName { get; set; } = null!;
}