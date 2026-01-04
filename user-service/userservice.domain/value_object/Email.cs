using System.Net;
using user_service.userservice.api.CustomExceptionService;

namespace user_service.userservice.domain.value_object
{
    public record Email
    {
        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                throw new ValidationNotAccept("Invalid email");

            Value = value;
        }


    }
}
