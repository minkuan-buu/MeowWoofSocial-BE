using MeowWoofSocial.Business.Services.UserServices;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeowWoofSocial.Data.DTO.Custom;
using Microsoft.AspNetCore.Authorization;
using MeowWoofSocial.Business.Services.UserFollowingServices;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IUserFollowingServices _userFollowingServices;

        public UserController(IUserServices userServices, IUserFollowingServices userFollowingServices)
        {
            _userFollowingServices = userFollowingServices;
            _userServices = userServices;
        }

        [HttpGet("{UserId}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetUser(Guid UserId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _userServices.GetUserById(UserId, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("follow")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> FollowUser([FromBody] UserFollowingReqModel userFollowing)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _userFollowingServices.FollowUser(userFollowing, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
