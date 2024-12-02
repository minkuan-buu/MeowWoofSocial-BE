using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeowWoofSocial.Data.Entities;

namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class PetStoreResModel
    {
    }
    
    public class PetStoreCreateResModel
    {
            public Guid Id { get; set; }
            
            public Guid UserId { get; set; }

            public string Name { get; set; } = null!;

            public string Description { get; set; } = null!;

            public string Email { get; set; } = null!;

            public string Phone { get; set; } = null!;

            public string Status { get; set; } = null!;

            public DateTime CreateAt { get; set; }
    }
    
    public class PetStoreUpdateResModel
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Status { get; set; } = null!;

        public DateTime? UpdateAt { get; set; }
    }
    
    public class PetStoreDeleteResModel
    {
        public Guid Id { get; set; }
    }
    public class PetStoreAuthorResModel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = null!;
        
        public string Description { get; set; } = null!;
        
    }
    
    public class PetStoreProductAttachmentResModel
    {
        public Guid Id { get; set; }
        public string Attachment { get; set; } = null!;
    }
    public class PetStoreProductCreateResModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        
        public int TotalSales { get; set; }
        
        public PetStoreAuthorResModel Author { get; set; } = null!;
        
        public List<PetStoreProductAttachmentResModel> Attachments { get; set; } = new();
        
        public List<PetStoreProductItems> PetStoreProductItems { get; set; } = new();
        
        public ProductCategory Category { get; set; } = new();
    }

    public class ProductCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ParentCategoryModel? ParentCategory { get; set; }
    }
    
    public class ParentCategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
    public class PetStoreProductItems
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        
    }
    public class PetStoreProductUpdateResModel
    {
        public Guid Id { get; set; }

        public Guid PetStoreId { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; } = null!;
        
        public PetStoreAuthorResModel Author { get; set; } = null!;
        
        public List<PetStoreProductAttachmentResModel> Attachments { get; set; } = new();
        
        public List<PetStoreProductItems> PetStoreProductItems { get; set; } = new();
        
        public ProductCategory Category { get; set; } = new();

        public DateTime? UpdateAt { get; set; }
    }
    
    public class PetStoreProductDeleteResModel
    {
        public Guid Id { get; set; }
        
        public string Status { get; set; }
        
        public DateTime UpdateAt { get; set; }
        
    }

    public class GetAllPetStoreProductResModel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = null!;
        
        public List<PetStoreProductAttachmentResModel> Attachments { get; set; } = new();
        
        public decimal Price { get; set; }
        
        public int TotalSales { get; set; }
    }

    public class PetStoreServiceResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Attachment { get; set; } = null!;
        public decimal AverageRating { get; set; }
    }
}
