using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infastructure.Models;

namespace order_service.OrderService.Appilcation.DTOs
{
    public class RequestGetListOrder
    {
        public string? NameCustomer { get; set; }

        public string? OrderCode { get; set; }

        public string? PhoneNumber { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
