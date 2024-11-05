using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class TransactionResModel
    {
        public Guid Id { get; set; }
        public int RefId { get; set; }
        public decimal TotalPrice { get; set; }
        public string StatusPayment { get; set; } = null!;
    }
    
    public class OrderCreateResModel
    {
        public Guid Id { get; set; }
    }
    
    public class TransactionPendingResModel
    {
        public Guid Id { get; set; }
    }
    
    public class OrderResModel
    {
        public Guid Id { get; set; }
        public string RefId { get; set; }
        public List<OrderPetStore> PetStores { get; set; } = null!;
        public OrderUserAddress UserAddress { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }

    public class OrderPetStore
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public List<OrderDetailResModel> OrderDetails { get; set; } = new();
    }
    
    public class OrderDetailResModel
    {
        public Guid Id { get; set; }
        public string Attachment { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string ProductItemName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderUserAddress
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
