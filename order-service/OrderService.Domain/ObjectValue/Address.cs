namespace order_service.OrderService.Domain.ObjectValue
{
    public class Address
    {
        public string Value { get; private set; }

        public Address(string value)
        {
            if (value.Length > 100)
            {
                throw new ArgumentException("Address cannot be longer than 100 characters.");
            }
            Value = value;
        }
    }
}
