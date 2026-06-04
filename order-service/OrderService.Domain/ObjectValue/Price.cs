namespace order_service.OrderService.Domain.ObjectValue
{
    public class Price
    {
        public decimal Value { get; private set; }
        public Price(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Value must be non-negative.");
            }
            Value = amount;
        }
    }
}
