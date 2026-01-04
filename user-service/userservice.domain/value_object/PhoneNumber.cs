namespace user_service.userservice.domain.value_object
{
    public record PhoneNumber
    {
        public string Number { get; init; }
        public PhoneNumber(string number)
        {
            if (number.Length < 10 || number.Length > 15)
            {
                throw new ArgumentException("Phone number must be between 10 and 15 digits.");
            }

            Number = number;
        }

    }
}
