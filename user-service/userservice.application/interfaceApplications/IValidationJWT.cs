namespace user_service.userservice.application.interfaceApplications
{
    public interface IValidationJWT
    {
        string GetTypeToken(HttpContext httpContext);
    }
}
