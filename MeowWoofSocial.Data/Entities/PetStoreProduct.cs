using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PetStoreProduct
{
    public Guid Id { get; set; }

    public Guid PetStoreId { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual PetStore PetStore { get; set; } = null!;

    public virtual ICollection<PetStoreProductAttachment> PetStoreProductAttachments { get; set; } = new List<PetStoreProductAttachment>();

    public virtual ICollection<PetStoreProductItem> PetStoreProductItems { get; set; } = new List<PetStoreProductItem>();

    public virtual ICollection<PetStoreProductRating> PetStoreProductRatings { get; set; } = new List<PetStoreProductRating>();

    public virtual ICollection<ProductRating> ProductRatings { get; set; } = new List<ProductRating>();
}
