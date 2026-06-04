using order_service.OrderService.Domain.Enums;

namespace order_service.OrderService.Application.DTOs
{
    public class RequestUpdateStatusOrder
    {
        public Guid IdOrder { get; set; }

        public OrderStatus Status { get; set; }
    }
}
