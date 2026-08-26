namespace Foodly.Contracts.Events
{
    public class ReservedOrderSuccess
    {
        public Guid IdOrder { get; set; }

        public string PaymentMethod { get; set; } = "PayOS";

        public Guid IdUser { get; set; }
    }
}
