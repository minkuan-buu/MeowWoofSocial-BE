using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? PostId { get; set; }

    public Guid? TradeId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime UpdateAt { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User User { get; set; } = null!;
}
