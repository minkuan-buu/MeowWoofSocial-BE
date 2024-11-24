using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Cart
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductItemId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual PetStoreProductItem ProductItem { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
