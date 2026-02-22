using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Appilcation;
using order_service.OrderService.Appilcation.Interface;
using order_service.OrderService.Domain.Aggregate;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Domain.OjectValue;
using order_service.OrderService.Infastructure.Models;

namespace order_service.OrderService.Infastructure.ServicesImplements
{
    public class UpdateStatusOrders : IUpdateStatusOrders
    {
        private readonly FoodOrderContext _db;

        public UpdateStatusOrders(FoodOrderContext foodOrderContext)
        {
            _db = foodOrderContext;
        }

        public async Task<bool> Excute(RequestUpdateStatusOrder request)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(s => s.Id == request.IdOrder);
            //var orderAggregate = new OrdersAggregate(order.Id, order.UserId, order.CartId, Enum.Parse<OrderStatusPayment>(order.Status), new Price(order.TotalAmount), order.ShippingFee, new DiscountValue(order.DiscountAmount), new Price(order.FinalAmount), Enum.Parse<PaymentMethod>(order.PaymentMethod!), order.CreatedAt, order.UpdatedAt ?? order.CreatedAt, null, null, null);


            if (order == null) return false;

            if (request.Status == OrderStatus.CANCELLED)
            {
                order.Status = OrderStatusPayment.CANCELLED.ToString();
                order.OrderStatus = request.Status.ToString();
            }
            else
            {
                order.OrderStatus = request.Status.ToString();
            }

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
