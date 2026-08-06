using cart_service.CartService.API.gRPC;
using cart_service.CartService.Application.DTOs;
using cart_service.CartService.Application.Services;
using cart_service.CartService.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using productService.API.Protos;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace cart_service.CartService.API.CartControllers
{
    [Route("api/cart")]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    [ApiController]
    public class FoodCartController : ControllerBase
    {
        private readonly FoodProductsDbContext _db;
        private readonly ICreateNewCart _cart;
        private readonly CartServiceClient _product;
        private readonly IUpdateCartFood _updateCart;
        private readonly IGetCartForUser _viewCart;

        public FoodCartController(FoodProductsDbContext foodProductsDbContext, ICreateNewCart createNewCart, CartServiceClient cartServiceClient, IUpdateCartFood updateCartFood, IGetCartForUser getCartForUser)
        {
            _db = foodProductsDbContext;
            _cart = createNewCart;
            _product = cartServiceClient;
            _updateCart = updateCartFood;
            _viewCart = getCartForUser;
        }

        [HttpGet("test")]
        public IActionResult test()
        {

            var result = _db.Carts.ToList();
            return Ok(result);
        }

     
        [HttpPost]
        public async Task<IActionResult> TestCreateCart([FromBody] RequestCreateNewCartUser request, CancellationToken cancellationToken)
        {
            var idCart = await _cart.Execute(request, cancellationToken);
            return Ok(idCart);
        }

     
        [HttpPost("update-cart")]
        public async Task<IActionResult> TestAddProductIntoCart([FromBody] RequestUpdateCartFood request)
        {
            await _updateCart.Excute(request);
            return Ok();
        }

    
        
        [HttpGet("user-cart/{idUser}")]
        public async Task<IActionResult> GetCartUser([FromRoute] Guid idUser, CancellationToken cancellationToken)
        {

            var cart = await _viewCart.Execute(idUser, cancellationToken);
            return Ok(cart);
        }
    }
}
