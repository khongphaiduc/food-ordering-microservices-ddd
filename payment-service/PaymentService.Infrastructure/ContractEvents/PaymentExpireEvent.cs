namespace Foodly.Contracts.Events
{
    public class PaymentExpireEvent
    {
        public string OrderCode { get; set; } = null!;

        public DateTime Happen { get; set; }
    }
}
