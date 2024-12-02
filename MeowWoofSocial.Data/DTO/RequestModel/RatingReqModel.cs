namespace MeowWoofSocial.Data.DTO.RequestModel;

public class RatingReqModel
{
    public Guid ProductItemId { get; set; }
    public decimal StarRating { get; set; }
    public string? Comment { get; set; }
}