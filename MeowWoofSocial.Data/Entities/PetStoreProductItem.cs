using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PetStoreProductItem
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PetStoreProduct Product { get; set; } = null!;
}
