namespace user_service.userservice.application.dtos
{
    public class RequestInfoAddressUser
    {
        public Guid IdUser { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

    }
}
