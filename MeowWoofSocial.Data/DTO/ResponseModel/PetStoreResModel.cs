using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class PetStoreResModel
    {
    }
    
    public class PetStoreCreateResModel
    {
            public Guid UserId { get; set; }

            public string Name { get; set; } = null!;

            public string Description { get; set; } = null!;

            public string Email { get; set; } = null!;

            public string Phone { get; set; } = null!;

            public string Status { get; set; } = null!;

            public DateTime CreateAt { get; set; }
    }
}
