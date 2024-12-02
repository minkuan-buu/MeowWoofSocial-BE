using MeowWoofSocial.Business.Services.CartServices;
using MeowWoofSocial.Business.Services.UserServices;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeowWoofSocial.Data.DTO.Custom;
using Microsoft.AspNetCore.Authorization;
using MeowWoofSocial.Business.Services.UserFollowingServices;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServices _cartServices;

        public CartController(ICartServices cartServices)
        {
            _cartServices = cartServices;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetCarts()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _cartServices.GetCart(token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> AddToCarts([FromBody] CartReqModel cartReqModel)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _cartServices.AddToCart(cartReqModel, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> UpdateCarts([FromBody] CartReqModel cartReqModel)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _cartServices.UpdateCart(cartReqModel, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> DeleteCarts(Guid id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _cartServices.DeleteCart(id, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
