using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PetCareBooking
{
    public Guid Id { get; set; }

    public Guid PetStoreId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreateAt { get; set; }

    public string Status { get; set; } = null!;

    public Guid PetCareCategoryId { get; set; }

    public Guid OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<PetCareBookingDetail> PetCareBookingDetails { get; set; } = new List<PetCareBookingDetail>();

    public virtual PetCareCategory PetCareCategory { get; set; } = null!;

    public virtual PetStore PetStore { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
