namespace user_service.userservice.domain.value_object
{
    public record PostalCode
    {
        public string Value { get; }
        public PostalCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 10)
                throw new ArgumentException("Invalid postal code");

            Value = value.Trim();
        }
    }

}
