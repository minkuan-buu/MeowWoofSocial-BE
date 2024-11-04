using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class UserResModel
    {
    }

    public class CreateHashPasswordModel
    {
        public byte[] Salt { get; set; } = null!;
        public byte[] HashedPassword { get; set; } = null!;
    }

    public class UserLoginResModel
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Avartar { get; set; }
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class UserProfileResModel
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }

    public class UserProfilePageResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFollow { get; set; }
        public List<UserFollowResModel> Following { get; set; } = new();
        public List<UserFollowResModel> Follower { get; set; } = new();
    }

    public class UserFollowResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public bool? IsFollow { get; set; } = null;
    }

    public class UpdateUserProfileResModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;
        
        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public DateTime? UpdateAt { get; set; }
    }

    public class UpdateUserAvartarResModel
    {
        public Guid Id { get; set; }

        public string? Avatar { get; set; }

        public DateTime? UpdateAt { get; set; }
    }

    public class UserAddressCreateResModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;
    }

    public class UserAddressUpdateResModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;

        public DateTime UpdateAt { get; set; }
    }

    public class UserAddressSetDefaultResModel
    {
        public Guid Id { get; set; }

        public string Status { get; set; } = null!;

        public DateTime UpdateAt { get; set; }
    }
}
