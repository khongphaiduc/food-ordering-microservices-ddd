namespace Foodly.Contracts.Events
{
    public class OrderPaymentTimeoutEvent
    {
        public string OrderCode { get; set; } = null!;
    }
}
