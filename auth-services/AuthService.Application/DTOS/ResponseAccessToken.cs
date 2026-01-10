namespace auth_services.AuthService.Application.DTOS
{
    public class ResponseAccessToken
    {
        public bool IsSuccess { get; set; }
        public string? TokenType { get; set; } = null!;

        public string? TokenValue { get; set; } = null!;

        public DateTime? CreateAt
        {
            get; set;

        }

        public DateTime? ExpireAt
        {
            get; set;
        }

        public string? Message { get; set; }

    }
}
