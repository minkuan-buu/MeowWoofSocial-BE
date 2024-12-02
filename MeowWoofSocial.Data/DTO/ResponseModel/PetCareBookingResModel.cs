namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class PetCareBookingResModel
    {
    }
    public class PetCareBookingCreateResModel
    {
        public Guid Id { get; set; }
        public Guid PetStoreId { get; set; }
        public Guid UserId { get; set; }
        public Guid PetCareCategoryId { get; set; }
        public DateTime CreateAt { get; set; }
        public string Status { get; set; }
        public List<PetCareBookingDetailCreateResModel> PetCareBookingDetails { get; set; } = new();
    }
    
    public class PetCareBookingDetailCreateResModel
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid PetId { get; set; }
        public string TypeTakeCare { get; set; }
        public string TypeOfDisease { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
    }
}