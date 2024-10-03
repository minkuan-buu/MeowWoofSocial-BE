using MeowWoofSocial.Business.Services.PostServices;
using MeowWoofSocial.Business.Services.UserFollowingServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserFollowingController : ControllerBase
    {
        private readonly IUserFollowingServices _userFollowingServices;

        public UserFollowingController(IUserFollowingServices userFollowingServices)
        {
            _userFollowingServices = userFollowingServices;
        }

        [HttpPost("follow-user")]
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
