namespace food_service.ProductService.Domain.ValueOject
{
    public record Name
    {
        public string Value { get; set; }

        public Name(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Name cannot be empty");
            }

            Value = value;
        }
    }
}
