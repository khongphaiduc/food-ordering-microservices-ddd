namespace user_service.userservice.application.dtos
{
    public class RequestPersonalInforUsers
    {
        public Guid IdUser { get; set; }

        public string Name { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

    }
}
