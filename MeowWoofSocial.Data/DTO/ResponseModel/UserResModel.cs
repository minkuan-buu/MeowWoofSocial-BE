using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTEducation.Data.DTO.ResponseModel
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
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class UserProfileResModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
