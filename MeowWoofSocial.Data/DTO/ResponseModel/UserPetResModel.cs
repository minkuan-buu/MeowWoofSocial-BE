namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class UserPetResModel
    {
    }
    
    public class UserPetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Breed { get; set; } = null!;

        public string Age { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public decimal Weight { get; set; }

        public string Attachment { get; set; } = null!;
    }
    
    public class UserPetCreateResMdoel
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Breed { get; set; } = null!;

        public string Age { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public decimal Weight { get; set; }

        public string Attachment { get; set; } = null!;
        
        public DateTime CreateAt { get; set; }
    }
    
    public class UserPetUpdateResMdoel
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Breed { get; set; } = null!;

        public string Age { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public decimal Weight { get; set; }

        public string Attachment { get; set; } = null!;
        
        public DateTime UpdateAt { get; set; }
    }
}