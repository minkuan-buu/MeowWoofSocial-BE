using MeowWoofSocial.Business.Services.PetStoreServices;
using MeowWoofSocial.Business.Services.ReactionServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [ApiController]
    public class PetStoreController : ControllerBase
    {
        private readonly IPetStoreServices _petStoreServices;

        public PetStoreController(IPetStoreServices petStoreServices)
        {
            _petStoreServices = petStoreServices;
        }
        [HttpPost("create-pet-store")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> CreatePetStore([FromBody] PetStoreCreateReqModel petStore)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _petStoreServices.CreatePetStore(petStore, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
