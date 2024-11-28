using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PetStoreProductRating
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public decimal Rating { get; set; }

    public string? Comment { get; set; }

    public virtual PetStoreProduct Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
