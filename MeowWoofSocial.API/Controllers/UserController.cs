using MeowWoofSocial.Business.Services.UserServices;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeowWoofSocial.Data.DTO.Custom;
using Microsoft.AspNetCore.Authorization;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("{UserId}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetUser(Guid UserId)
        {
            try
            {
                var result = await _userServices.GetUserById(UserId);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
