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
    public Guid Id { get; set; }
    public string Attachment { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductItemName { get; set; } = null!;
}