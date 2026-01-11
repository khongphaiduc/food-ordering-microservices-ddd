namespace auth_services.AuthService.Application.DTOS
{
    public class ResponseLoginUser
    {
        public bool IsLoginSuccessful { get; set; }
        public Guid Id { get; set; }

        public string Email { get; set; }

        public TokenResponse? AccessToken { get; set; }

        public TokenResponse? RefreshToken { get; set; }

        public string? Message { get; set; }
    }
}
