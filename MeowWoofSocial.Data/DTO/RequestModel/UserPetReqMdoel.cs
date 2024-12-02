using Microsoft.AspNetCore.Http;

namespace MeowWoofSocial.Data.DTO.RequestModel
{
    public class UserPetReqMdoel
    {
    }
    
    public class UserPetCreateReqMdoel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Breed { get; set; } = null!;

        public string Age { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public decimal Weight { get; set; }

        public IFormFile Attachment { get; set; } = null!;
    }
    
    public class UserPetUpdateReqMdoel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Breed { get; set; } = null!;

        public string Age { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public decimal Weight { get; set; }

        public IFormFile Attachment { get; set; } = null!;
    }
}