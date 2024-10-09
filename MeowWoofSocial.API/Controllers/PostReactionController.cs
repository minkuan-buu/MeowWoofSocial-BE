using MeowWoofSocial.Business.Services.PostServices;
using MeowWoofSocial.Business.Services.ReactionServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/react")]
    [ApiController]
    public class PostReactionController : ControllerBase
    {
        private readonly IPostReactionServices _postReactionServices;

        public PostReactionController(IPostReactionServices postReactionServices)
        {
            _postReactionServices = postReactionServices;
        }

        [HttpPost("create-comment")]
        public async Task<IActionResult> CreateComment([FromForm] CommentCreateReqModel commentReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postReactionServices.CreateComment(commentReq, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create-reaction")]
        public async Task<IActionResult> CreateFeeling([FromBody] FeelingCreateReqModel feelingReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postReactionServices.CreateFeeling(feelingReq, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
