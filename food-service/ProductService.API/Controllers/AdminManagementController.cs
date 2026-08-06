using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;
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
        private readonly IAdminProductDailyInventory _adminInventory;
        private readonly IGetProductDailyInventory _getProductDailyInventory;

        public AdminManagementController(IUpdateProduct updateProduct, ICreateNewProduct createNewProduct, ICreateNewCategory createNewCategory, IUpdateCategory updateCategory, IAdminProductDailyInventory adminInventory, IGetProductDailyInventory getProductDailyInventory, ILogger<AdminManagementController> logger)
        {
            _iAddNewCategory = createNewCategory;
            _iAddNewProduct = createNewProduct;
            _iUpdateCategory = updateCategory;
            _logger = logger;
            _updateProduct = updateProduct;
            _adminInventory = adminInventory;
            _getProductDailyInventory = getProductDailyInventory;
        }

        // tested
        [HttpPost("products")]
        public async Task<ActionResult> CreateNewProduct([FromForm] CreateNewProductDTO request)
        {
            var result = await _iAddNewProduct.ExecuteAsync(request);
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
            var result = await _iAddNewCategory.ExecuteAsync(request);
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
            var result = await _iUpdateCategory.ExecuteAsync(request);

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
            await _updateProduct.Execute(updateProduct);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "AccessToken", Roles = "Admin")]
        [HttpGet("inventory")]
        public async Task<IActionResult> GetProductDailyInventory(
            [FromQuery] RequestGetProductDailyInventory request,
            CancellationToken cancellationToken)
        {
            var result = await _getProductDailyInventory.ExecuteAsync(request, cancellationToken);

            return Ok(new
            {
                list = result.Items,
                totalProduct = result.TotalCount,
                inventoryDate = result.InventoryDate,
                summary = result.Summary,
                pageIndex = result.PageIndex,
                pageSize = result.PageSize
            });
        }

        [Authorize(AuthenticationSchemes = "AccessToken", Roles = "Admin")]
        [HttpPost("inventory")]
        public async Task<IActionResult> CreateProductDailyInventory(
            [FromBody] CreateProductDailyInventoryRequest request,
            CancellationToken cancellationToken)
        {
            if (request.ProductId == Guid.Empty)
            {
                return BadRequest(new { message = "ProductId is required." });
            }

            var result = await _adminInventory.CreateAsync(request, cancellationToken);

            return result.Status switch
            {
                AdminInventoryOperationStatus.Success => StatusCode(
                    StatusCodes.Status201Created,
                    new { result.Message, result.Inventory }),
                AdminInventoryOperationStatus.ProductNotFound => NotFound(
                    new { result.Message }),
                AdminInventoryOperationStatus.InventoryAlreadyExists => Conflict(
                    new { result.Message }),
                _ => BadRequest(new { result.Message })
            };
        }

        [Authorize(AuthenticationSchemes = "AccessToken", Roles = "Admin")]
        [HttpPost("inventory/restock")]
        public async Task<IActionResult> RestockProductDailyInventory(
            [FromBody] RestockProductDailyInventoryRequest request,
            CancellationToken cancellationToken)
        {
            if (request.ProductId == Guid.Empty)
            {
                return BadRequest(new { message = "ProductId is required." });
            }

            var result = await _adminInventory.RestockAsync(request, cancellationToken);

            return result.Status switch
            {
                AdminInventoryOperationStatus.Success => Ok(
                    new { result.Message, result.Inventory }),
                AdminInventoryOperationStatus.InventoryNotFound => NotFound(
                    new { result.Message }),
                AdminInventoryOperationStatus.QuantityLimitExceeded => Conflict(
                    new { result.Message }),
                _ => BadRequest(new { result.Message })
            };
        }


    }
}
