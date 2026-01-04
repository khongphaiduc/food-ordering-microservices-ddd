using user_service.userservice.domain.value_object;

namespace user_service.userservice.domain.entity
{
    public class AddressUserEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public StreetAddress AddressLine1 { get; set; } = null!;
        public string? AddressLine2 { get; set; }
        public CityName City { get; set; } = null!;
        public string? District { get; set; }
        public PostalCode? PostalCode { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
