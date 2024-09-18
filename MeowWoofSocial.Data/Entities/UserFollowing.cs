using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class UserFollowing
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid FollowerId { get; set; }

    public string Status { get; set; } = null!;

    public virtual User Follower { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
