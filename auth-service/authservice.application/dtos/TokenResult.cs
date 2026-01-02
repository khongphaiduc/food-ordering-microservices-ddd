namespace auth_service.authservice.application.dtos
{
    public class TokenResult
    {
        public string TypeToken { get; set; }
        public string Token { get; set; }
        public DateTime TimeExpire { get; set; }
        public DateTime TimeCreate { get; set; }    

    }
}
