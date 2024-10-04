using MeowWoofSocial.Business.Services.PostServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IPostServices _postServices;

        public CommentController(IPostServices postServices)
        {
            _postServices = postServices;
        }

        [HttpPost("create-comment")]
        public async Task<IActionResult> CreateComment([FromForm] CommentCreateReqModel commentReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postServices.CreateComment(commentReq, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
