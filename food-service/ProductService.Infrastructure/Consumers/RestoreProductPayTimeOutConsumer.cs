using food_service.ProductService.API.gRPC;
using food_service.ProductService.Infrastructure.Models;
using Foodly.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using order_service.OrderService.API.Proto;

namespace food_service.ProductService.Infrastructure.Consumers
{
    public class RestoreProductPayTimeOutConsumer : IConsumer<OrderPaymentTimeoutEvent>

    {
        private readonly OrderByOrderCodeGrpc.OrderByOrderCodeGrpcClient _orderClient;
        private readonly FoodProductsDbContext _db;
        private readonly ILogger<RestoreProductPayTimeOutConsumer> _logger;

        public RestoreProductPayTimeOutConsumer(FoodProductsDbContext context, OrderByOrderCodeGrpc.OrderByOrderCodeGrpcClient orderByOrderCodeGrpcClient, ILogger<RestoreProductPayTimeOutConsumer> logger)
        {
            _db = context;
            _orderClient = orderByOrderCodeGrpcClient;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderPaymentTimeoutEvent> context)
        {
           

            try
            {

                var order = await _orderClient
                    .ViewOrderDetailByOrderCodeAsync(
                        new RequestOrderByOrderCode
                        {
                            OrderCode = context.Message.OrderCode
                        });

                if (order == null)
                {
                    _logger.LogWarning(
                        "Order not found: {OrderCode}",
                        context.Message.OrderCode);

                    return;
                }


              
                var dto = DateTimeOffset.Parse(order.DateTime);

            
                var localTime = TimeZoneInfo.ConvertTime(dto, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

             
                var inventoryDate = DateOnly.FromDateTime(localTime.DateTime);


                foreach (var item in order.OrderItems)
                {
                    var productId = Guid.Parse(item.ProductId);

                    var quantity = item.Quantity;

                    if (quantity <= 0)
                        continue;

                    await _db.Database.ExecuteSqlInterpolatedAsync($"""
                         UPDATE public.product_daily_inventories
                        SET
                            remaining_quantity = remaining_quantity + {quantity},
                            sold_quantity = sold_quantity - {quantity},
                            updated_at = NOW()
                        WHERE product_id = {productId}
                        AND inventory_date = {inventoryDate}
                        """);
                }



                _logger.LogInformation(
                    "Successfully restored inventory for OrderCode: {OrderCode}",
                    context.Message.OrderCode);
            }
            catch (Exception ex)
            {

              

                _logger.LogError(
                    ex,
                    "Failed to restore inventory for OrderCode: {OrderCode}",
                    context.Message.OrderCode);

                throw;
            }
        }
    }
}