using order_service.OrderService.Domain.Enums;
using System.Net;

namespace order_service.OrderService.Application.DTOs
{

    public class OrderHistoryPagination
    {
        public List<ResponseListOrderOfUser> orderHistory { get; set; } = new List<ResponseListOrderOfUser>();

        public int TotalPages { get; set; }
    }

    public class ResponseListOrderOfUser
    {
        public Guid IDOrder { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public OrderStatusPayment OrderStatusPayment { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }


        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreateAt { get; set; }
    }
}
