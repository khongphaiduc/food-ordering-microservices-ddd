using cart_service.CartService.Domain.Interface;
using Foodly.Contracts.Events;
using MassTransit;

namespace cart_service.CartService.Infrastructure.Consumers
{
    public class CheckOutCart : IConsumer<CheckOutCartEvent>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IPublishEndpoint _ipublic;
        private readonly ILogger<CheckOutCart> _logger;

        public CheckOutCart(ICartRepository cartRepository, IPublishEndpoint publishEndpoint, ILogger<CheckOutCart> logger)
        {
            _cartRepo = cartRepository;
            _ipublic = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CheckOutCartEvent> context)
        {
            try
            {
                var result = await _cartRepo.CheckOutCartAsync(context.Message.IdCart);
                if (result)
                {
                    _logger.LogInformation("Cart checked out successfully for IdCart: {IdCart}", context.Message.IdCart);
                }
                else
                {
                    _logger.LogWarning("Failed to check out cart for IdCart: {IdCart}", context.Message.IdCart);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while checking out the cart for IdCart: {IdCart}", context.Message.IdCart + $"Bug is :{ex.Message}");
                throw;
            }

        }
    }

}
