namespace auth_services.AuthService.Application.DTOS
{
    public class TokenResponse
    {
        public string TokenType { get; set; } = null!;

        public string TokenValue { get; set; } = null!;

        public DateTime CreateAt
        {
            get; set;

        }

        public DateTime ExpireAt
        {
            get; set;
        }


    }
}