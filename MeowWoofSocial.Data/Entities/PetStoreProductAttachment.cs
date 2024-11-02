using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PetStoreProductAttachment
{
    public Guid Id { get; set; }

    public Guid PetStoreProductId { get; set; }

    public string Attachment { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public virtual PetStoreProduct PetStoreProduct { get; set; } = null!;
}
