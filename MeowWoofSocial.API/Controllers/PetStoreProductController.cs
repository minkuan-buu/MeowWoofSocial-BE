using MeowWoofSocial.Business.Services.PetStoreProductServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MeowWoofSocial.API.Controllers;

[Route("api/stores")]
[ApiController]
public class PetStoreProductController : ControllerBase
{
    private readonly IPetStoreProductServices _petStoreProductServices;

    public PetStoreProductController(IPetStoreProductServices petStoreProductServices)
    {
        _petStoreProductServices = petStoreProductServices;
    }
    
    [HttpPost("product")]
    [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
    public async Task<IActionResult> CreatePetStoreProduct([FromForm] PetStoreProductCreateReqModel petStoreProduct, [FromForm] string petstoreproductItem)
    {
        
        try
        {
            var petstoreproductItems = JsonConvert.DeserializeObject<List<PetStoreProductItemsReqModel>>(petstoreproductItem);
            petStoreProduct.PetStoreProductItems = petstoreproductItems;
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _petStoreProductServices.CreatePetStoreProduct(petStoreProduct, token);
            return Ok(result);
        }
        catch (CustomException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut("product")]
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
    
    [HttpDelete("product")]
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
    
    [HttpGet("products")]
    [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
    public async Task<IActionResult> GetUserPost([FromQuery] PetStoreProductReq petStoreProductReq)
    {
        var result = await _petStoreProductServices.GetAllPetStoreProduct(petStoreProductReq);
        return Ok(result);
    }
}