using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PetCareBookingDetail
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid PetId { get; set; }

    public string TypeTakeCare { get; set; } = null!;

    public string? TypeOfDisease { get; set; }

    public string Status { get; set; } = null!;

    public virtual PetCareBooking Booking { get; set; } = null!;

    public virtual UserPet Pet { get; set; } = null!;
}
