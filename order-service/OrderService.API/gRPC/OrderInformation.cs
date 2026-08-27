using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using order_service.OrderService.API.Proto;
using order_service.OrderService.Infrastructure.Models;

namespace order_service.OrderService.API.gRPC
{
    public class OrderInformation : OrderGrpc.OrderGrpcBase
    {
        private readonly FoodOrderContext _db;
        private readonly ILogger<OrderInformation> _logger;

        public OrderInformation(FoodOrderContext foodOrderContext, ILogger<OrderInformation> logger)
        {
            _db = foodOrderContext;
            _logger = logger;

        }

        public override async Task<ResponseOrder> ViewOrderDetail(RequestOrder request, ServerCallContext context)
        {

            try
            {
                if (Guid.TryParse(request.IdOrder, out Guid IdOrder))
                {
                    var order = await _db.Orders.Include(s => s.OrderItems).FirstOrDefaultAsync(o => o.Id == IdOrder && o.OrderStatus == "PENDING");

                    if (order == null) return new ResponseOrder
                    {
                        IdOrder = request.IdOrder,
                        MessageStatus = "Order not found or Order is handled",
                    };

                    var orderItem = order.OrderItems.Select(oi => new Proto.OrderItem
                    {
                        ProductId = oi.ProductId.ToString(),
                        Quantity = oi.Quantity
                    }).ToList();

                    var response = new ResponseOrder
                    {
                        IdOrder = order.Id.ToString(),
                        PaymentMethod = order.PaymentMethod,
                        OrderCode = order.OrderCode,
                        Amount = (long)order.FinalAmount,
                        MessageStatus = order.OrderStatus,
                        OrderItems = { orderItem }
                    };

                    return response;
                }
                return new ResponseOrder
                {
                    IdOrder = request.IdOrder,
                    MessageStatus = "Invalid Order ID",
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order details for Order ID: {OrderId}", request.IdOrder);
                throw;
            }

        }
    }
}
