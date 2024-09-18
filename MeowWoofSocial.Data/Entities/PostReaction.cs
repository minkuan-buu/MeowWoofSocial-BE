using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PostReaction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public string Type { get; set; } = null!;

    public string? Content { get; set; }

    public string? Attachment { get; set; }

    public string? TypeReact { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User User { get; set; } = null!;
}
