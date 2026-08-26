namespace Foodly.Contracts.Events
{
    public class CreatedOrderEvent
    {
        public Guid IdOrder { get; set; }
        public string PaymentMethod { get; set; } = "PayOS";

        public Guid IdUser { get; set; }
    }
}
