namespace Foodly.Contracts.Events
{
    public class PaymentFailed
    {
        public string OrderCode { get; set; } = null!;

        public DateTime Happen { get; set; }
    }
}
