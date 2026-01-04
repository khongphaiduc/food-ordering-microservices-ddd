namespace user_service.userservice.domain.value_object
{
    public record CityName
    {
        public string Value { get; }
        public CityName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("City name cannot be empty");

            Value = value.Trim();
        }
    }
}
