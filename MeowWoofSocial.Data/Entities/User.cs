using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Avartar { get; set; }

    public byte[] Password { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PetCareBooking> PetCareBookings { get; set; } = new List<PetCareBooking>();

    public virtual ICollection<PetStoreRating> PetStoreRatings { get; set; } = new List<PetStoreRating>();

    public virtual ICollection<PetStore> PetStores { get; set; } = new List<PetStore>();

    public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();

    public virtual ICollection<PostStored> PostStoreds { get; set; } = new List<PostStored>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<ProductRating> ProductRatings { get; set; } = new List<ProductRating>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    public virtual ICollection<UserBankInformation> UserBankInformations { get; set; } = new List<UserBankInformation>();

    public virtual ICollection<UserFollowing> UserFollowingFollowers { get; set; } = new List<UserFollowing>();

    public virtual ICollection<UserFollowing> UserFollowingUsers { get; set; } = new List<UserFollowing>();

    public virtual ICollection<UserPet> UserPets { get; set; } = new List<UserPet>();
}
