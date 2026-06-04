namespace auth_services.AuthService.Infrastructure.IntegrationContracts
{
    public class RegisterNotificationMessage
    {

        public string Name { get; set; }

        public string Email { get; set; }

        public string TypeService { get; set; } = "Email";

    }
}
