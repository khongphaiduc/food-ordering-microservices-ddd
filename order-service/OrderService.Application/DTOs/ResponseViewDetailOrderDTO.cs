using order_service.OrderService.Domain.Enums;

namespace order_service.OrderService.Application.DTOs
{
    public class ResponseViewDetailOrderDTO
    {
        public OrderStatusPayment OrderStatusPayments { get; set; }

        public OrderStatus orderStatus { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreateAt { get; set; }
        public List<ResponseViewDetailOrderItemDTO>? OrderItems { get; set; }    

        public OrderDeliveryDTO? orderDeliveryDTO { get; set; }
    }


    public class ResponseViewDetailOrderItemDTO
    {
        public string ProductName { get; set; }
        public string Variantname { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }


    public class OrderDeliveryDTO
    {
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public string Note { get; set; }
    }


}
