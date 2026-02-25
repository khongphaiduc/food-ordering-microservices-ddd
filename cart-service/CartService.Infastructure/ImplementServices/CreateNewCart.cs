using cart_service.CartService.Application.DTOs;
using cart_service.CartService.Application.Services;
using cart_service.CartService.Domain.Aggregate;
using cart_service.CartService.Domain.Interface;
using System.Threading.Tasks;

namespace cart_service.CartService.Infastructure.ImplementServices
{
    public class CreateNewCart : ICreateNewCart
    {
        private readonly ICartRepository _cartRepo;
        private readonly ILogger<CreateNewCart> _logger;

        public CreateNewCart(ICartRepository cartRepository, ILogger<CreateNewCart> logger)
        {
            _cartRepo = cartRepository;
            _logger = logger;
        }

        public async Task<Guid> Execute(RequestCreateNewCartUser request)
        {

            var cartAggregate = CartAggregate.CreateNewCart(request.UserId);

            var idCart = await _cartRepo.CreateCartAsync(cartAggregate);

            return idCart;
        }

    }
}
