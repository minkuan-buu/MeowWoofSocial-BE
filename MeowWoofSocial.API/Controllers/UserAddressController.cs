using MeowWoofSocial.Business.Services.PetStoreServices;
using MeowWoofSocial.Business.Services.UserAddressServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressServices _userAddressServices;

        public UserAddressController(IUserAddressServices userAddressServices)
        {
            _userAddressServices = userAddressServices;
        }

        [HttpPost("create-user-address")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> CreatePetStore([FromBody] UserAddressCreateReqModel userAddressReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _userAddressServices.CreateUserAddress(userAddressReq, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
