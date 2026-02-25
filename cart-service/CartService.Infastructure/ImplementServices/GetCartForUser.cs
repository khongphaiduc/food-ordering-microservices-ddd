using cart_service.CartService.API.gRPC;
using cart_service.CartService.Application.DTOInternal;
using cart_service.CartService.Application.DTOs;
using cart_service.CartService.Application.Services;
using cart_service.CartService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace cart_service.CartService.Infastructure.ImplementServices
{
    public class GetCartForUser : IGetCartForUser
    {
        private readonly FoodProductsDbContext _db;
        private readonly CartServiceClient _grpcCart;
        private readonly ICreateNewCart _createCart;
        private readonly ILogger<GetCartForUser> _logger;

        public GetCartForUser(FoodProductsDbContext foodProductsDbContext, CartServiceClient cartServiceClient, ICreateNewCart createNewCart, ILogger<GetCartForUser> logger)
        {
            _db = foodProductsDbContext;
            _grpcCart = cartServiceClient;
            _createCart = createNewCart;
            _logger = logger;
        }
        #region get cart if not exist create new cart
        public async Task<ResponseViewCartUser> Execute(Guid idUser)
        {

            var cartUser = await _db.Carts.Include(s => s.CartItems).Where(s => s.Status == "ACTIVE").FirstOrDefaultAsync(c => c.UserId == idUser);

            // create new cart 
            if (cartUser == null)
            {
                var idcart = await _createCart.Execute(new RequestCreateNewCartUser
                {
                    UserId = idUser,
                });

                _logger.LogInformation($"Creat new cart for user {idUser}");


                return new ResponseViewCartUser
                {
                    IdCart = idcart,
                    TotalCart = 0,
                    cartItems = new List<CartItems>()
                };

            }



            var cart = new ResponseViewCartUser
            {
                IdCart = cartUser.Id,
                TotalCart = cartUser.TotalPrice,
                cartItems = cartUser.CartItems.Select(s => new CartItems
                {
                    CartItemId = s.Id,
                    IdProduct = s.ProductId,
                    IdVariant = s.VariantId ?? Guid.Empty,
                    Quantity = s.Quantity,
                    Price = s.UnitPrice,
                    NameProduct = s.ProductName,
                    NameVariant = s.VariantName
                }).ToList()
            };


            var listIDProduct = cart.cartItems.Select(s => new GetUrlImageProduct
            {
                IdProduct = s.IdProduct
            }).ToList();



            var listImage = await _grpcCart.GetProductImage(listIDProduct);

            var imageDict = listImage.GroupBy(x => x.IdProduct).ToDictionary(g => g.Key, g => g.First().UrlImage);

            foreach (var item in cart.cartItems)
            {
                item.UrlImage = imageDict.TryGetValue(item.IdProduct, out var url) ? url : null;
            }

            return cart;

        }
        #endregion 
    }
}
