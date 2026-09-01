using Elastic.Clients.Elasticsearch.Requests;
using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Threading.Tasks;

namespace food_service.ProductService.API.Controllers
{
    //[EnableRateLimiting("rateFix")]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IGetListProduct _iListProduct;
        private readonly IViewDetailProduct _iViewDetailProduct;
        private readonly IProductRecommendationService _recommendationProduct;
        private readonly IGetListCategory _getListCategory;
        private readonly IGetProductDailyInventory _getProductDailyInventory;



        public ProductsController( IGetListCategory getListCategory, IGetProductDailyInventory getProductDailyInventory, IGetListProduct listProduct, IViewDetailProduct viewDetailProduct, IProductRecommendationService productRecommendationService)
        {
            _iListProduct = listProduct;
            _iViewDetailProduct = viewDetailProduct;
            _recommendationProduct = productRecommendationService;
            _getListCategory = getListCategory;
            _getProductDailyInventory = getProductDailyInventory;

        }


        // tested
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetListProduct([FromQuery] RequestGetListProduct request)
        {
            var listProduct = await _iListProduct.ExecuteAsync(request);
            var totalProduct = await _iListProduct.TotalProdut();
            return Ok(new { list = listProduct, totalProduct = totalProduct });
        }


        // AI Agent recommend food 
        [AllowAnonymous]
        [HttpGet("ai")]
        public async Task<IActionResult> GetListProductRecommendByAI()
        {

                var sampleProduct = await _iListProduct.ExecuteAsync(new RequestGetListProduct());
                return Ok(new { list = sampleProduct, totalProduct = sampleProduct.Count });
 
        }

        [AllowAnonymous]
        [HttpGet("category")]
        public async Task<IActionResult> GetListCategory()
        {
            var listCategory = await _getListCategory.Excute();
            return Ok(new { list = listCategory });
        }

        [AllowAnonymous]
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




        // tested
        [AllowAnonymous]
        [HttpGet("{idProduct}")]

        public async Task<IActionResult> ViewDetailProduct([FromRoute] Guid idProduct)
        {
            var detailProduct = await _iViewDetailProduct.ExecuteAsync(idProduct);

            if (detailProduct != null)
            {
                return Ok(detailProduct);
            }
            else
            {
                return NotFound($"Not Found Product Id : {idProduct}");
            }
        }


        [AllowAnonymous]
        [HttpGet("recommendation/{idCategory}")]
        public async Task<IActionResult> GetProductRecommendation([FromRoute] Guid idCategory)
        {
            var listProductRecommendation = await _recommendationProduct.ExecuteAsync(idCategory);
            return Ok(listProductRecommendation);

        }
    }
}
