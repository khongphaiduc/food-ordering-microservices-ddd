using order_service.OrderService.API.GlobalException;
using System.Numerics;
using System.Text.RegularExpressions;

namespace order_service.OrderService.Domain.ObjectValue
{
    public class Phone
    {
        public string Value { get; private set; }

        public Phone(string value)
        {
            if (value.Length > 11 || value.Length < 10)
            {
                throw new PhoneException("The phone number must be between 10 and 11 digits.");
            }
            if (!Regex.IsMatch(value, @"^\d+$"))
            {
                throw new PhoneException("The phone number must contain only digits.");
            }
            Value = value;
        }
    }
}
