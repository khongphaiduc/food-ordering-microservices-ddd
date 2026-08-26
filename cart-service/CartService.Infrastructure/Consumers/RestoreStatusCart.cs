using cart_service.CartService.Domain.Interface;
using Foodly.Contracts.Events;
using MassTransit;

namespace cart_service.CartService.Infrastructure.Consumers
{
    public class RestoreStatusCart : IConsumer<RestoreStatusCartEvent>
    {
        private readonly ICartRepository _cart;
        private readonly ILogger<RestoreStatusCart> _logger;

        public RestoreStatusCart(ICartRepository cartRepository, ILogger<RestoreStatusCart> logger)
        {
            _cart = cartRepository;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<RestoreStatusCartEvent> context)
        {
            try
            {
                var result = await _cart.RestoreOutCartAsync(context.Message.IdCart);
                if (result)
                {
                    _logger.LogInformation($"Cart with ID {context.Message.IdCart} restored successfully.");
                }
                else
                {
                    _logger.LogWarning($"Failed to restore cart with ID {context.Message.IdCart}. Cart may not exist or is already active.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while restoring cart with ID {context.Message.IdCart}." + $"Bug : {ex.Message}");
                throw;
            }
        }
    }
}
