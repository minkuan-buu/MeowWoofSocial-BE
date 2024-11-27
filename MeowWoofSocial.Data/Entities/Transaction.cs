using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public int OrderPaymentRefId { get; set; }

    public string PaymentLinkId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? FinishedTransactionAt { get; set; }

    public string? TransactionReference { get; set; }

    public virtual Order Order { get; set; } = null!;
}
