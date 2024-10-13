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
    public class PostController : ControllerBase
    {
        private readonly IPostServices _postServices;

        public PostController(IPostServices postServices)
        {
            _postServices = postServices;
        }

        [HttpPost("create-post")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> CreatePost([FromForm] PostCreateReqModel post)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postServices.CreatePost(post, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("news-feed")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetNewsFeed([FromQuery]NewsFeedReq newsFeedReq)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _postServices.GetNewsFeed(token, newsFeedReq);
            return Ok(result);
        }

        [HttpPut("update-post")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> UpdatePost([FromForm] PostUpdateReqModel postUpdateReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postServices.UpdatePost(postUpdateReq, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _postServices.GetPostByID(id, token);
            return Ok(result);
        }

        [HttpDelete("delete-post")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> DeletePost([FromBody] PostRemoveReqModel postRemoveReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _postServices.RemovePost(postRemoveReq, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
