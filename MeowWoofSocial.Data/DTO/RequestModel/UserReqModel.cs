using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.DTO.RequestModel
{
    public class UserReqModel
    {
    }
    public class UserLoginReqModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class UserRegisterReqModel()
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = null!;
        [RegularExpression(@"^\d{10}$", ErrorMessage = "The PhoneNumber must be exactly 10 digits.")]
        public string? Phone { get; set; }
    }
}
