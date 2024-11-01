using MeowWoofSocial.Business.Services.PetStoreProductServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers;

public class PetStoreProductController : Controller
{
    private readonly IPetStoreProductServices _petStoreProductServices;

    public PetStoreProductController(IPetStoreProductServices petStoreProductServices)
    {
        _petStoreProductServices = petStoreProductServices;
    }
    
    [HttpPost("create-pet-store-product")]
    [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
    public async Task<IActionResult> CreatePetStore([FromBody] PetStoreProductCreateReqModel petStoreProduct)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _petStoreProductServices.CreatePetStoreProduct(petStoreProduct, token);
            return Ok(result);
        }
        catch (CustomException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut("update-pet-store-product")]
    [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
    public async Task<IActionResult> UpdatePetStore([FromBody] PetStoreProductUpdateReqModel petStoreProductUpdateReq)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _petStoreProductServices.UpdatePetStoreProduct(petStoreProductUpdateReq, token);
            return Ok(result);
        }
        catch (CustomException ex)
        {
            return BadRequest(new { message = ex.Message });
            
        }
    }
    
    [HttpDelete("delete-pet-store-product")]
    [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
    public async Task<IActionResult> DeletePetStore([FromBody] PetStoreProductDeleteReqModel PetStoreProductDeleteReq)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _petStoreProductServices.DeletePetStoreProduct(PetStoreProductDeleteReq, token);
            return Ok(result);
        }
        catch (CustomException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}