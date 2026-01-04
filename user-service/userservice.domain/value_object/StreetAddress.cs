namespace user_service.userservice.domain.value_object
{
    public record StreetAddress
    {
        public string Value { get; }
        public StreetAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Street address cannot be empty");
            if (value.Length > 200)
                throw new ArgumentException("Street address too long");

            Value = value.Trim();
        }
    }
}
