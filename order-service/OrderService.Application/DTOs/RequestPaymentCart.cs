namespace order_service.OrderService.Application.DTOs
{
    public class RequestPaymentCart
    {
        public Guid IdCart { get; set; }

        public int PaymentMethod { get; set; }

        public Guid IdAddress { get; set; }
    }
}
