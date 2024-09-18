using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class OrderDetail
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductItemId { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual PetStoreProductItem ProductItem { get; set; } = null!;
}
