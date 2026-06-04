using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Application.Services;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infrastructure.Models;

namespace order_service.OrderService.Infrastructure.ServicesImplements
{
    #region view detail order for customer
    public class GetViewDetailOrder : IGetViewDetailOrder
    {
        private readonly FoodOrderContext _db;

        public GetViewDetailOrder(FoodOrderContext foodOrderContext)
        {
            _db = foodOrderContext;
        }

        public async Task<ResponseViewDetailOrderDTO> Execute(RequestViewOrderDetail request)
        {
            var order = await _db.Orders.Include(s => s.OrderItems).Include(s => s.OrderDelivery).FirstOrDefaultAsync(s => s.Id == request.IdOrder && s.UserId == request.IdUser);

            if (order == null) return new ResponseViewDetailOrderDTO();

            var s = new ResponseViewDetailOrderDTO
            {
                OrderStatusPayments = Enum.Parse<OrderStatusPayment>(order.Status),
                orderStatus = Enum.Parse<OrderStatus>(order.OrderStatus),
                ShippingFee = order.ShippingFee,
                TotalPrice = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                PaymentMethod = Enum.Parse<PaymentMethod>(order.PaymentMethod!),
                CreateAt = order.CreatedAt,
                OrderItems = order.OrderItems.Select(oi => new ResponseViewDetailOrderItemDTO
                {
                    ProductName = oi.ProductName,
                    Variantname = oi.VariantName ?? "L?i hi?n th?",
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                }).ToList(),

            };

            if (order.OrderDelivery != null)
            {
                s.orderDeliveryDTO = new OrderDeliveryDTO
                {
                    DeliveryAddress = order.OrderDelivery.Address ?? "None",
                    RecipientName = order.OrderDelivery.ReceiverName,
                    RecipientPhone = order.OrderDelivery.Phone,
                };
            }

            Console.WriteLine();

            return s;
        }
    }
    #endregion
}
