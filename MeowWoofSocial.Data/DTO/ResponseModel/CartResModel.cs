namespace MeowWoofSocial.Data.DTO.ResponseModel;

public class CartResModel
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = null!;
    public List<CartDetailResModel> CartItems { get; set; } = new();
}

public class CartDetailResModel
{
    public Guid CartId { get; set; }
    public Guid ProductItemId { get; set; }
    public string Attachment { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductItemName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = null!;
}