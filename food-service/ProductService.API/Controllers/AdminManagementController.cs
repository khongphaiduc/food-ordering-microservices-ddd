using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace food_service.ProductService.API.Controllers
{
    [Route("api/admin")]
    //[Authorize(AuthenticationSchemes = "AccessToken")]
    [ApiController]
    public class AdminManagementController : ControllerBase
    {
        private readonly ICreateNewCategory _iAddNewCategory;
        private readonly ICreateNewProduct _iAddNewProduct;
        private readonly IUpdateCategory _iUpdateCategory;
        private readonly ILogger<AdminManagementController> _logger;
        private readonly IUpdateProduct _updateProduct;

        public AdminManagementController(IUpdateProduct updateProduct, ICreateNewProduct createNewProduct, ICreateNewCategory createNewCategory, IUpdateCategory updateCategory, ILogger<AdminManagementController> logger)
        {
            _iAddNewCategory = createNewCategory;
            _iAddNewProduct = createNewProduct;
            _iUpdateCategory = updateCategory;
            _logger = logger;
            _updateProduct = updateProduct;
        }


        // đã test
        [HttpPost("products")]
        public async Task<ActionResult> CreateNewProduct([FromForm] CreateNewProducDTO request)
        {
            var result = await _iAddNewProduct.ExcuteAsync(request);
            if (result)
            {
                return Ok(new { message = "Create new product successful" });
            }
            else
            {
                return BadRequest(new { message = "Failed to create product.", time = DateTime.Now });
            }
        }

        //tested
        [HttpPost("categories")]
        public async Task<IActionResult> CreateNewCategory([FromBody] CreateNewCategoryDTO request)
        {
            var result = await _iAddNewCategory.ExcuteAsync(request);
            if (result)
            {
                return Ok(new { message = "Create new category successful" });
            }
            else
            {
                return BadRequest(new { message = "Failed to create category.", time = DateTime.Now });
            }
        }





        // tested
        [HttpPut("categories")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO request)
        {
            var result = await _iUpdateCategory.ExcuteAsync(request);

            if (result)
            {
                return Ok(new { message = "Update category successful" });
            }
            else
            {
                return BadRequest(new { message = "Failed to update category.", time = DateTime.Now });
            }
        }

        [HttpPut("products")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDTO updateProduct)
        {
            await _updateProduct.Excute(updateProduct);
            return Ok();
        }


    }
}
