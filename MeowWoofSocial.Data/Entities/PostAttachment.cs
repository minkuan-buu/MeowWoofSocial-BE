using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class PostAttachment
{
    public Guid Id { get; set; }

    public Guid PostId { get; set; }

    public string? Attachment { get; set; }

    public string Status { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}
