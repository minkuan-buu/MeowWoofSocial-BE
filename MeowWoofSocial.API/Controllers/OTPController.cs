using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Business.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Business.Services.OTPServices;

namespace MeowWoofSocial.API.Controllers
{
    [ApiController]
    [Route("api/otp")]
    public class OTPController : ControllerBase
    {
        private readonly IOTPServices _otpServices;
        public OTPController(IOTPServices otpServices)
        {
            _otpServices = otpServices;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOTP([FromBody] string Email)
        {
            try
            {
                var Result = await _otpServices.SendOTPEmailRequest(Email);
                return Ok(Result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOTP([FromBody] UserVerifyOTPReqModel OTP)
        {
            try
            {
                var Result = await _otpServices.VerifyOTPCode(OTP.Email, OTP.OTPCode);
                return Ok(Result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
