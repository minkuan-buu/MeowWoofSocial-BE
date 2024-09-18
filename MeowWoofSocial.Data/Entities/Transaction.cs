using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
