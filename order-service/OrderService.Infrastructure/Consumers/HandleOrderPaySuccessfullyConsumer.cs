using Foodly.Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infrastructure.Models;
using order_service.OrderService.Infrastructure.OrderRealTime;

namespace order_service.OrderService.Infrastructure.Consumers
{
    public class HandleOrderPaySuccessfullyConsumer : IConsumer<PaySuccessfullyEvent>
    {
        private FoodOrderContext _db;
        private readonly IHubContext<OrderOfUser> _hubOrder;
        private readonly ILogger<HandleOrderPaySuccessfullyConsumer> _logger;

        public HandleOrderPaySuccessfullyConsumer(IHubContext<OrderOfUser> hubContext, FoodOrderContext foodOrderContext, ILogger<HandleOrderPaySuccessfullyConsumer> logger)
        {
            _db = foodOrderContext;
            _hubOrder = hubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaySuccessfullyEvent> context)
        {
            try
            {

                var affect = await _db.Database
                    .ExecuteSqlInterpolatedAsync(
                    $"UPDATE [dbo].[Orders] SET [Status] = {OrderStatusPayment.PAID.ToString()} WHERE [OrderCode] = {context.Message.OrderCode} AND [Status] = {OrderStatusPayment.PENDING.ToString()}");

                var order = _db.Orders.FirstOrDefault(s => s.OrderCode == context.Message.OrderCode);

                if (order != null)
                {
                    _logger.LogInformation($"IDUser Is :{order.UserId}");
                    await _hubOrder.Clients.User(order.UserId.ToString()).SendAsync("OrderPaySuccessfully", $"Amount :{order.FinalAmount}" + $"Time :{context.Message.Happen}");
                }
                else
                {
                    _logger.LogInformation($"Can't  get the id user");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Bug:{ex.Message}");
                throw;
            }
        }
    }
}
