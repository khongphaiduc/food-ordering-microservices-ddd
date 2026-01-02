namespace auth_service.authservice.api.CustomExceptionSerives
{
    public class RevokeTokenFailException : Exception
    {
        public RevokeTokenFailException(string message) : base(message)
        {
        }
    }
}
