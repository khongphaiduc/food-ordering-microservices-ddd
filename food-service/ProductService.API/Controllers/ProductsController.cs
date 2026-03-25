using Elastic.Clients.Elasticsearch.Requests;
using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IRecommenPersonalFood _recommentionAI;

        public ProductsController(IGetListCatgory getListCatgory, IGetListProduct listProduct, IViewDetailProduct viewDetailProduct, IProductRecommendationService productRecommendationService, IRecommenPersonalFood recommenPersonalFood)
        {
            _iListProduct = listProduct;
            _iViewDetailProduct = viewDetailProduct;
            _recommentionProduct = productRecommendationService;
            _getListCategory = getListCatgory;
            _recommentionAI = recommenPersonalFood;

        }


        // đã test
        [HttpGet]
        public async Task<IActionResult> GetListProduct([FromQuery] RequestGetListProduct request)
        {
            var listProduct = await _iListProduct.ExecuteAsync(request);
            var totalProduct = await _iListProduct.TotalProdut();
            return Ok(new { list = listProduct, totalProduct = totalProduct });
        }


        [HttpGet("ai/{IdUser}")]
        public async Task<IActionResult> GetListProductRecommendByAI([FromRoute] Guid IdUser)
        {

            var listProduct = await _recommentionAI.Execute(IdUser);
            return Ok(new { list = listProduct, totalProduct = listProduct.Count });
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

        [HttpGet("recommendation/{idCategory}")]
        public async Task<IActionResult> GetProductRecommendation([FromRoute] Guid idCategory)
        {
            var listProductRecommendation = await _recommentionProduct.ExecuteAsync(idCategory);
            return Ok(listProductRecommendation);

        }




    }
}
