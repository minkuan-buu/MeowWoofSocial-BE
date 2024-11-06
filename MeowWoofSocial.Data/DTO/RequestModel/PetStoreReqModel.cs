using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeowWoofSocial.Data.DTO.ResponseModel;
using Microsoft.AspNetCore.Http;

namespace MeowWoofSocial.Data.DTO.RequestModel
{
    public class PetStoreReqModel
    {
    }

    public class PetStoreCreateReqModel
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;
        
    }
    
    public class PetStoreUpdateReqModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;
        
    }
    
    public class PetStoreDeleteReqModel
    {
        public Guid PetStoreId { get; set; }
    }
    
    public class PetStoreProductCreateReqModel
    {
        public Guid PetStoreId { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; } = null!;
        
        public List<IFormFile>? Attachment { get; set; }

        public List<PetStoreProductItemsReqModel> PetStoreProductItems { get; set; } = new();
    }
    
    public class PetStoreProductItemsReqModel
    {
        public string Name { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
    
    public class PetStoreProductUpdateReqModel
    {
        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; } = null!;
        
        public List<IFormFile>? Attachment { get; set; }

        public List<PetStoreProductItemsReqModel> PetStoreProductItems { get; set; } = new();

    }
    
    public class PetStoreProductDeleteReqModel
    {
        public Guid PetStoreProductId { get; set; }
    }
    
    public class PetStoreProductReq
    {
        public int PageSize { get; set; }
        
        public Guid? lastPetStoreProductId { get; set; } = null;
        
        public int? loadedPetStoreProduct { get; set; } = null;
    }
}
