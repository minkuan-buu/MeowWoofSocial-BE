using Microsoft.AspNetCore.Http;
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
        //[Required]
        public string Name { get; set; } = null!;
        //[Required]
        public string Email { get; set; } = null!;
        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = null!;
        //[RegularExpression(@"^\d{10}$", ErrorMessage = "The PhoneNumber must be exactly 10 digits.")]
        public string? Phone { get; set; }
    }

    public class UserFollowingReqModel()
    {
        public Guid UserId { get; set; }
    }
    public class UpdateUserProfileReqModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;
        
        public string Name { get; set; } = null!;
  
        public string Phone { get; set; } = null!;
        
    }

    public class UpdateUserAvartarReqModel
    {
        public Guid Id { get; set; }

        public IFormFile? Avartar { get; set; }

    }

    public class  UserAddressCreateReqModel
    {
        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;
    }

    public class UserAddressUpdateReqModel
    {
        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Address { get; set; } = null!;
    }

    public class UserAddressSetDefaultReqModel
    {
        public Guid Id { get; set; }
    }

    public class UserAddressDeleteReqModel
    {
        public Guid UserAddressId { get; set; }
    }

    public class UserResetPasswordReqModel
    {
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }

    public class UserVerifyOTPReqModel
    {
        public string Email { get; set; } = null!;
        public string OTPCode { get; set; } = null!;
    }
}
