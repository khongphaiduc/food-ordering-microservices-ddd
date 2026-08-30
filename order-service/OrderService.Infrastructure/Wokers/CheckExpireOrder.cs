using Foodly.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infrastructure.Models;

namespace order_service.OrderService.Infrastructure.Workers
{
    public class CheckExpireOrder : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CheckExpireOrder> _logger;


        public CheckExpireOrder(
            IBus bus,
            IServiceProvider serviceProvider,
            ILogger<CheckExpireOrder> logger)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();


                    var db = scope.ServiceProvider
                        .GetRequiredService<FoodOrderContext>();



                    var expireOrders = await db.Orders
                        .Where(x =>
                            x.Status == OrderStatusPayment.PENDING.ToString()
                            &&
                            x.CreatedAt <= DateTime.UtcNow.AddMinutes(-5))
                        .Take(100)
                        .ToListAsync(stoppingToken);



                    if (expireOrders.Count == 0)
                    {
                        await Task.Delay(
                            TimeSpan.FromSeconds(30),
                            stoppingToken);

                        continue;
                    }




                    foreach (var order in expireOrders)
                    {
                        order.Status = OrderStatusPayment.CANCELLED.ToString();
                        order.OrderStatus = OrderStatus.CANCELLED.ToString();
                    }




                    await db.SaveChangesAsync(stoppingToken);



                    foreach (var order in expireOrders)
                    {
                        await _bus.Publish(
                            new OrderPaymentTimeoutEvent
                            {
                                OrderCode = order.OrderCode
                            },
                            stoppingToken);


                        _logger.LogInformation(
                            "Published OrderPaymentTimeoutEvent for order {OrderCode}",
                            order.OrderCode);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Check expire order worker error");
                }




                await Task.Delay(
                    TimeSpan.FromSeconds(30),
                    stoppingToken);
            }
        }
    }
}