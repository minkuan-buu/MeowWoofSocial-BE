using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Report
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Type { get; set; }

    public Guid? PostId { get; set; }

    public Guid? CommentId { get; set; }

    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public bool IsProcessed { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual PostReaction? Comment { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User User { get; set; } = null!;
}
