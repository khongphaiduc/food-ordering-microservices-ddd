namespace food_service.ProductService.Domain.ValueOject
{
    public record Price
    {
        public decimal Value { get; init; }

        public Price(decimal value)
        {

            if (value < 0)
            {
                throw new ArgumentException("Price cannot be negative");
            }

            Value = value;
        }

    }
}
