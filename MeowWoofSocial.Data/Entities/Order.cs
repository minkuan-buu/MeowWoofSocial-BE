using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string RefId { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
