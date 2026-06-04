namespace order_service.OrderService.Application.DTOs
{
    public class ResponseCreateNewOrder
    {
        public Guid IdOrder { get; set; }
        public string OrderCode { get; set; } = string.Empty;

        public decimal FinalAmount { get; set; }
        public bool Status { get; set; }
    }
}
