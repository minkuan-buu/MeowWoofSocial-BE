using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PetStore
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string TypeStore { get; set; } = null!;

    public string? Attachment { get; set; }

    public virtual ICollection<PetCareBooking> PetCareBookings { get; set; } = new List<PetCareBooking>();

    public virtual ICollection<PetStoreProduct> PetStoreProducts { get; set; } = new List<PetStoreProduct>();

    public virtual ICollection<PetStoreRating> PetStoreRatings { get; set; } = new List<PetStoreRating>();

    public virtual User User { get; set; } = null!;
}
