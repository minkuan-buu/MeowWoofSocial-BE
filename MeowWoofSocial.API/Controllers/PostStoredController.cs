using MeowWoofSocial.Business.Services.PostServices;
using MeowWoofSocial.Business.Services.UserServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostStoredController : ControllerBase
    {
        private readonly IPostServices _postServices;

        public PostStoredController(IPostServices postServices)
        {
            _postServices = postServices;
        }

        [HttpPost("store")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> CreatePostStore([FromBody] PostIdReqModel postReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postServices.StorePost(postReq.PostId, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
