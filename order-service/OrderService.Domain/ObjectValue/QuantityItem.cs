namespace order_service.OrderService.Domain.ObjectValue
{
    public class QuantityItem
    {
        public int Value { get; private set; }

        public QuantityItem(int value)
        {
            if (value <=0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            Value = value;
        }
    }
}
