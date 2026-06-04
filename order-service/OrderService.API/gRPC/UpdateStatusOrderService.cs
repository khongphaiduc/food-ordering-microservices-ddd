using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infrastructure.Models;
using OrderService.API.Proto;

namespace order_service.OrderService.API.gRPC
{
    public class UpdateStatusOrderService : OrderServiceUpdateStatusGrpc.OrderServiceUpdateStatusGrpcBase
    {
        private readonly FoodOrderContext _db;

        public UpdateStatusOrderService(FoodOrderContext db)
        {
            _db = db;
        }

        #region update status order
        public override async Task<ResponseOrderUpdateStatus> UpdateStatusOrder(RequestOrderUpdateStatus request, ServerCallContext context)
        {

            var Order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderCode == request.OrderCode);

            if (Order == null) return new ResponseOrderUpdateStatus
            {
                MessageUpdate = "Order not found",
                StatusUpdateOrder = false
            };


            Order.Status = OrderStatusPayment.PAID.ToString();
            Order.UpdatedAt = DateTime.UtcNow;

            var resultAfterUpdate = await _db.SaveChangesAsync();

            return resultAfterUpdate > 0 ? new ResponseOrderUpdateStatus
            {
                MessageUpdate = "Order status updated to PAID successfully",
                StatusUpdateOrder = true,
                UserID = Order.UserId.ToString()
            } : new ResponseOrderUpdateStatus
            {
                MessageUpdate = "Failed to update order status",
                StatusUpdateOrder = false
            };
        }
        #endregion 
    }
}
