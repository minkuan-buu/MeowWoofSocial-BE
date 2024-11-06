using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class UserPet
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Breed { get; set; } = null!;

    public string Age { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public decimal Weight { get; set; }

    public string Attachment { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<PetCareBookingDetail> PetCareBookingDetails { get; set; } = new List<PetCareBookingDetail>();

    public virtual User User { get; set; } = null!;
}
