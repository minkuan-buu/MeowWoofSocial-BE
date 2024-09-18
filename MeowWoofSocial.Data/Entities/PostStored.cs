using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PostStored
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
