using Microsoft.Identity.Client;
using order_service.OrderService.Domain.Enums;

namespace order_service.OrderService.Application.DTOs
{

    public class ViewManagementOrder
    {
        public List<ViewOrderDTO>? listOrderDTOs { get; set; }
        public int ConfirmationCount { get; set; }
        public int PreparingCount { get; set; }
        public int DeliveringCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }

    }

    public class ViewOrderDTO
    {
        public Guid IdOrder { get; set; }

        public string NameCustomer { get; set; } = null!;

        public string OrderCode { get; set; } = null!;

        public PaymentMethod PaymentMethod { get; set; }

        public OrderStatusPayment OrderStatusPayment { get; set; }

        public OrderStatus orderStatus { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreateAt { get; set; }
    }
}
