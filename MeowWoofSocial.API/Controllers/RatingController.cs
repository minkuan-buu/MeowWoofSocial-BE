using MeowWoofSocial.Business.Services.RatingServices;
using MeowWoofSocial.Business.Services.UserPetServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/rating")]
    [ApiController]
    public class RatingController : Controller
    {
        private readonly IRatingServices _ratingServices;

        public RatingController(IRatingServices ratingServices)
        {
            _ratingServices = ratingServices;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> Rating([FromBody] List<RatingReqModel> ratingReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _ratingServices.RatingOrder(token, ratingReq);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("product/{productId}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetRating(Guid productId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _ratingServices.GetProductRating(token, productId);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}