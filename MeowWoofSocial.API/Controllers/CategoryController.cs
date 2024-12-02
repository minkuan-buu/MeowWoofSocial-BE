using MeowWoofSocial.Business.Services.CategoryServices;
using MeowWoofSocial.Business.Services.RatingServices;
using MeowWoofSocial.Business.Services.UserPetServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryServices _categoryServices;

        public CategoryController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetCategory()
        {
            try
            {
                var result = await _categoryServices.GetCategories();
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("filter/all")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetFilterAllCategory()
        {
            try
            {
                var result = await _categoryServices.GetFilterCategories();
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("filter/{categoryId}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetFilterCategory(Guid categoryId)
        {
            try
            {
                var result = await _categoryServices.GetFilterCategories(categoryId);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}