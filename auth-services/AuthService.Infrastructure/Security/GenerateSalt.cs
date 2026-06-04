using auth_services.AuthService.Application.Interfaces;

namespace auth_services.AuthService.Infrastructure.Security
{
    public class GenerateSalt : IGenerateSalt
    {
        string IGenerateSalt.GenerateSalt()
        {
           return Guid.NewGuid().ToString().Replace("-", "");   
        }
    }
}
