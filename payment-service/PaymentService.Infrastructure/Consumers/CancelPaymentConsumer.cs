using Foodly.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using payment_service.PaymentService.Domain.Enums;
using payment_service.PaymentService.Infrastructure.Models;

namespace payment_service.PaymentService.Infrastructure.Consumers
{
    public class CancelPaymentConsumer : IConsumer<OrderPaymentTimeoutEvent>
    {
        private readonly FoodPaymentContext _db;
        private readonly ILogger<CancelPaymentConsumer> _logger;

        public CancelPaymentConsumer(FoodPaymentContext foodPaymentContext, ILogger<CancelPaymentConsumer> logger)
        {
            _db = foodPaymentContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderPaymentTimeoutEvent> context)
        {
            try
            {
                var affected = await _db.Database.ExecuteSqlInterpolatedAsync(
     $@"
    UPDATE [dbo].[Payments]
    SET 
        [Status] = {PaymentStatus.Cancelled.ToString()},
        [UpdatedAt] = {DateTime.UtcNow}
    WHERE 
        [OrderCode] = {context.Message.OrderCode}
        AND [Status] = {PaymentStatus.Pending.ToString()}
    ");


                if (affected == 1)
                {
                    _logger.LogInformation("Payment {OrderCode} cancelled", context.Message.OrderCode);


                }
                _logger.LogWarning("Payment {OrderCode} cannot be cancelled because status changed", context.Message.OrderCode);
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
