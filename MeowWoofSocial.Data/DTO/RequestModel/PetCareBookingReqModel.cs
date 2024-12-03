namespace MeowWoofSocial.Data.DTO.RequestModel
{

    public class PetCareBookingReqModel
    {
    }

    public class PetCareBookingCreateReqModel
    {
        public Guid PetStoreId { get; set; }
        public List<PetCareBookingDetailCreateReqModel> PetCareBookingDetails { get; set; } = new();
    }
    
    public class PetCareBookingDetailCreateReqModel
    {
        public Guid PetId { get; set; }
        public string TypeTakeCare { get; set; }
        public string TypeOfDisease { get; set; }
        public DateTime BookingDate { get; set; }
    }
    
    public class PetCareBookingUpdateReqModel
    {
        public Guid Id { get; set; }
        public List<PetCareBookingDetailCreateReqModel> PetCareBookingDetails { get; set; } = new();
    }
}