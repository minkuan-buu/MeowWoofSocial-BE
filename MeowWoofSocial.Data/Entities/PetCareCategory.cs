using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PetCareCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Attachment { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<PetCareBooking> PetCareBookings { get; set; } = new List<PetCareBooking>();
}
