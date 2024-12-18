using MeowWoofSocial.Business.Services.UserPetServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/pet")]
    [ApiController]
    public class UserPetController : Controller
    {
        private readonly IUserPetServices _userPetServices;

        public UserPetController(IUserPetServices userPetServices)
        {
            _userPetServices = userPetServices;
        }

        [HttpPost("create-user-pet")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> CreatePetStore([FromForm] UserPetCreateReqMdoel userPetReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _userPetServices.CreateUserPet(userPetReq, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("update-user-pet")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> UpdateUserPet([FromForm] UserPetUpdateReqMdoel userPetReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _userPetServices.UpdateUserPet(userPetReq, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> DeleteUserPet(Guid id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _userPetServices.DeleteUserPet(id, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPetStoreById(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _userPetServices.GetUserPetByUserID(id, token);
            return Ok(result);
        }
    }
}