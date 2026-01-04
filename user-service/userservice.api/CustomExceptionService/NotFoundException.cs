namespace user_service.userservice.api.CustomExceptionService
{
    public class NotFoundException : Exception
    {

        public NotFoundException(string? message) : base(message)
        {
        }
    }
}
