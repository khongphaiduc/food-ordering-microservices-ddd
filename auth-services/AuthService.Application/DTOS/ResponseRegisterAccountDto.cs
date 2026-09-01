namespace auth_services.AuthService.Application.DTOS
{
    public class ResponseRegisterAccountDto
    {
        public bool Status { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
