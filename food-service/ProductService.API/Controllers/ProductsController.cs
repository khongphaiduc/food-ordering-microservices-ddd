using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace food_service.ProductService.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IGetListProduct _iListProduct;
        private readonly IViewDetailProduct _iViewDetailProduct;

        public ProductsController(IGetListProduct listProduct, IViewDetailProduct viewDetailProduct)
        {
            _iListProduct = listProduct;
            _iViewDetailProduct = viewDetailProduct;
        }


        // đã test
        [HttpGet]
        public async Task<IActionResult> GetListProduct([FromQuery] RequestGetListProduct request)
        {
            var listProduct = await _iListProduct.ExecuteAsync(request);

            return Ok(listProduct);
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

    }
}
