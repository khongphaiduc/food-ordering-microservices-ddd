namespace user_service.userservice.api.CustomExceptionService
{
    public class ValidationNotAccept : Exception
    {
        public ValidationNotAccept(string? message) : base(message)
        {

        }
    }
}
