using order_service.OrderService.Domain.Enums;

namespace order_service.OrderService.Application.DTOs
{
    public class StaffViewDetailOrderDTO
    {
        public Guid OrderId { get; set; }

        public Guid IdUser { get; set; }

        public string SnapshotNameCustomer { get; set; } = "Khách L?";
        public string SnapshotPhoneNumber { get; set; } = null!;

        public string OrderCode { get; set; } = null!;


        public decimal TotalAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public OrderStatusPayment OrderStatusPayment { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public decimal ShipmentAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreateAt { get; set; }

        public List<OrderItemDetail> orderItemDetail { get; set; } = null!;
        public OrderDeliveryInfor? orderDeliveryInfor { get; set; }
    }

    public class OrderItemDetail
    {
        public string NameProduct { get; set; }

        public string NameVariant { get; set; }

        public decimal PricePerProduct { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

    }

    public class OrderDeliveryInfor
    {
        public Guid OrderId { get; set; }
        public string ReciveName { get; set; } = "Khách";

        public string RecivePhoneNumber { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string Note { get; set; } = "Không có note";
    }
}
