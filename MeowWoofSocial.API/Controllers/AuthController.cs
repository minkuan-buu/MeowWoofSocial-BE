using MeowWoofSocial.Business.Services.UserServices;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeowWoofSocial.Data.DTO.Custom;
using Microsoft.AspNetCore.Authorization;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public AuthController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginReqModel User)
        {
            try
            {
                var result = await _userServices.LoginUser(User);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterReqModel newUser)
        {
            try
            {
                var result = await _userServices.RegisterUser(newUser);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordReqModel User)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var Result = await _userServices.ResetPassword(User, token);
                return Ok(Result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
