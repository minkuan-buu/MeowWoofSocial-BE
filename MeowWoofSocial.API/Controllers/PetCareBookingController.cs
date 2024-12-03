using MeowWoofSocial.Business.Services.PetCareBookingServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/petcarebooking")]
    [ApiController]
    public class PetCareBookingController : ControllerBase
    {
        private readonly IPetCareBookingServices _petCareBookingServices;
        
        public PetCareBookingController(IPetCareBookingServices petCareBookingServices)
        {
            _petCareBookingServices = petCareBookingServices;
        }
        
        [HttpPost("create-pet-care-booking")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> CreatePetStore([FromBody] PetCareBookingCreateReqModel petCareBookingCreateReq)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _petCareBookingServices.CreatePetCareBooking(petCareBookingCreateReq, token);
                return Ok(new { redirectUrl = result });
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> CancelPetCareBooking(Guid id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _petCareBookingServices.CancelPetCareBooking(id, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}