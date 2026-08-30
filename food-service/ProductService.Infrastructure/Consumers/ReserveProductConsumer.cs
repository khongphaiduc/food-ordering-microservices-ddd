using food_service.ProductService.API.gRPC;
using food_service.ProductService.Infrastructure.Models;
using Foodly.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace food_service.ProductService.Infrastructure.Consumers
{
    public class ReserveProductConsumer : IConsumer<CreatedOrderEvent>
    {

        private readonly LoadOrder _gRPCOrder;
        private readonly FoodProductsDbContext _db;
        private readonly IPublishEndpoint _IpublishEvent;
        private readonly ILogger<ReserveProductConsumer> _logger;

        public ReserveProductConsumer(ILogger<ReserveProductConsumer> logger, LoadOrder loadOrder, FoodProductsDbContext foodProductsDbContext, IPublishEndpoint publishEndpoint)
        {
            _gRPCOrder = loadOrder;
            _db = foodProductsDbContext;
            _IpublishEvent = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreatedOrderEvent> context)
        {
            _logger.LogInformation("Start Consumer Reserver Product");

         

            try
            {
                var idOrder = context.Message.IdOrder;

                var order = await _gRPCOrder.LoadOrderAsync(idOrder);

                foreach (var item in order.OrderItems)
                {
                    var affectedRows = await _db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE public.product_daily_inventories
                SET
                    remaining_quantity = remaining_quantity - {item.Quantity},
                    sold_quantity = sold_quantity + {item.Quantity},
                    updated_at = NOW()
                WHERE remaining_quantity >= {item.Quantity}
                  AND inventory_date = CURRENT_DATE
                  AND product_id = {item.IdProduct}
                """);

                    // trừ kho thất bại 
                    if (affectedRows == 0)
                    {
                        await _IpublishEvent.Publish(new ReservedOrderFail
                        {
                            IdOrder = idOrder,
                            PaymentMethod = order.PaymentMethod,
                            IdUser = context.Message.IdUser
                        });

                        throw new InvalidOperationException(
                            $"Not enough inventory for product {item.IdProduct}");
                    }
                }

                
                // bắn event thành công cho thằng Payment Services;
                await _IpublishEvent.Publish(new ReservedOrderSuccess
                {
                    IdOrder = idOrder,
                    PaymentMethod = order.PaymentMethod,
                    IdUser = context.Message.IdUser
                });

                _logger.LogInformation("Handle Reduce Product Success");

            }
            catch
            {
               
                throw;
            }
        }
    }
}
