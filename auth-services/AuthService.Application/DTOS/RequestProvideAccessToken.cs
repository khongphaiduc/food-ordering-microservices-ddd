namespace auth_services.AuthService.Application.DTOS
{
    public class RequestProvideAccessToken
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
