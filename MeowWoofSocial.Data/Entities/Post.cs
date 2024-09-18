using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Post
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Type { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Attachment { get; set; } = null!;

    public string Hashtag { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PostAttachment> PostAttachments { get; set; } = new List<PostAttachment>();

    public virtual ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();

    public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();

    public virtual ICollection<PostStored> PostStoreds { get; set; } = new List<PostStored>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual User User { get; set; } = null!;
}
