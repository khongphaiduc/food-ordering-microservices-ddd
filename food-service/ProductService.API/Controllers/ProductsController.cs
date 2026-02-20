using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace food_service.ProductService.API.Controllers
{
    //[EnableRateLimiting("rateFix")]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IGetListProduct _iListProduct;
        private readonly IViewDetailProduct _iViewDetailProduct;
        private readonly IProductRecommendationService _recommentionProduct;
        private readonly IGetListCatgory _getListCategory;

        public ProductsController(IGetListCatgory getListCatgory, IGetListProduct listProduct, IViewDetailProduct viewDetailProduct, IProductRecommendationService productRecommendationService)
        {
            _iListProduct = listProduct;
            _iViewDetailProduct = viewDetailProduct;
            _recommentionProduct = productRecommendationService;
            _getListCategory = getListCatgory;
        }


        // đã test
        [HttpGet]
        public async Task<IActionResult> GetListProduct([FromQuery] RequestGetListProduct request)
        {
            var listProduct = await _iListProduct.ExecuteAsync(request);
            var totalProduct = await _iListProduct.TotalProdut();
            return Ok(new { list = listProduct, totalProduct = totalProduct });
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetListCategory()
        {
            var listCategory = await _getListCategory.Excute();
            return Ok(new { list = listCategory });
        }




        // đã test 
        [HttpGet("{idProduct}")]
        public async Task<IActionResult> ViewDetailProduct([FromRoute] Guid idProduct)
        {
            var detailProduct = await _iViewDetailProduct.ExcuteAsync(idProduct);

            if (detailProduct != null)
            {
                return Ok(detailProduct);
            }
            else
            {
                return NotFound($"Not Found Product Id : {idProduct}");
            }
        }

        [HttpGet("recommendation/{idCategory}")]
        public async Task<IActionResult> GetProductRecommendation([FromRoute] Guid idCategory)
        {
            var listProductRecommendation = await _recommentionProduct.ExecuteAsync(idCategory);
            return Ok(listProductRecommendation);

        }
    }
}
