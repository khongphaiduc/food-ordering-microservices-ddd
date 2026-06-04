namespace order_service.OrderService.Application.DTOs
{
    public class RequestViewOrderDetail
    {
        public Guid IdUser { get; set; }

        public Guid IdOrder { get; set; }
    }
}
