namespace user_service.UserService.API.Middlewares
{
    public class NotFoundUserException : Exception
    {
        public NotFoundUserException()
        {
        }

        public NotFoundUserException(string? message) : base(message)
        {
        }
    }
}
