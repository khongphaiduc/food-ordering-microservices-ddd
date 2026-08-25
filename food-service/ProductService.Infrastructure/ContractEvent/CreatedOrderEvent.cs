namespace food_service.ProductService.Infrastructure.ContractEvent
{
    public class CreatedOrderEvent
    {
        public Guid IdOrder { get; set; }
        public string PaymentMethod { get; set; } = "PayOS";
    }
}
