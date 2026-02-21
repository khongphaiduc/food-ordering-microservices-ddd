using CartService.API.Protos;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using order_service.OrderService.Appilcation.DTOs.DTOsInternal;
using order_service.OrderService.Domain.Enums;
using System.Threading.Tasks;

namespace order_service.OrderService.API.gRPC
{
    public class GetInformationOfCart
    {
        private readonly CartInforGrpc.CartInforGrpcClient _cartInforGrpcClient;
        private readonly ILogger<GetInformationOfCart> _logger;

        public GetInformationOfCart(CartInforGrpc.CartInforGrpcClient cartInforGrpcClient, ILogger<GetInformationOfCart> logger)
        {
            _cartInforGrpcClient = cartInforGrpcClient;
            _logger = logger;
        }

        #region get information of cart
        public async Task<CartDTOsInternal> Excute(Guid IdCart)
        {
            global::CartService.API.Protos.Cart cart = new();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    cart = await _cartInforGrpcClient.GetInformationCartAsync(new CartID { IdCart = IdCart.ToString() }, deadline: DateTime.UtcNow.AddSeconds(3));
                    break;
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable || ex.StatusCode == StatusCode.DeadlineExceeded)
                {
                    _logger.LogError(ex, $"Error occurred while calling gRPC to get cart information for Cart ID: {IdCart}. Attempt {i + 1} of 3.");
                    if (i == 2) throw;
                    await Task.Delay(200);
                }
            }

            if (cart == null || cart.IdCart == Guid.Empty.ToString())
            {
                return new CartDTOsInternal
                {
                    CartId = Guid.Empty,
                    CartItems = new List<CartItemDTOsInternal>(),
                    CartDiscounts = new List<CartDiscountDTOsInternal>(),
                    TotalPrice = 0
                };
            }
            return new CartDTOsInternal
            {

                CartId = Guid.Parse(cart.IdCart),
                UserId = Guid.Parse(cart.UserId),
                Status = OrderStatusPayment.PENDING,
                TotalPrice = cart.TotalPrice,
                CartItems = cart.CartItems.Select(x => new CartItemDTOsInternal
                {
                    CartId = Guid.Parse(x.CartId),
                    ProductId = Guid.Parse(x.ProductId),
                    ProductName = x.ProductName,
                    VariantId = string.IsNullOrEmpty(x.VariantId) ? null : Guid.Parse(x.VariantId),
                    VariantName = x.VariantName ?? null,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    TotalPrice = x.TotalPrice
                }).ToList(),
                CartDiscounts = cart.CartDiscounts.Select(x => new CartDiscountDTOsInternal
                {
                    CartId = Guid.Parse(x.CartId),
                    DiscountCode = x.DiscountCode,
                    DiscountAmount = x.DiscountAmount,
                    TotalDiscountAmount = x.AmountAfterDiscount
                }).ToList()

            };
        }
        #endregion



        #region change status cart
        public async Task<bool> ChangeStatusCart(Guid IdCart, StatusCart statusCart)
        {
            var result = await _cartInforGrpcClient.ChangeStatusCartAsync(new global::CartService.API.Protos.RequestChangeStatusCart
            {
                IdCart = IdCart.ToString(),
                StatusChange = statusCart.ToString()
            });
            if (result.Status)
            {
                _logger.LogInformation("Change status cart {IdCart} to {StatusChange}", IdCart, statusCart);
            }
            else
            {
                _logger.LogError("Failed to change status cart {IdCart} to {StatusChange}", IdCart, statusCart);
            }

            return result.Status;
        }
        #endregion
    }
}
