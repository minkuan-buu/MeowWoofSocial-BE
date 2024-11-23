using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Otp
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Code { get; set; } = null!;

    public bool IsUsed { get; set; }

    public DateTime ExpiredDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
