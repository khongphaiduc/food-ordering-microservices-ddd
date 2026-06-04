using auth_services.AuthService.Application.DTOS;

namespace auth_services.AuthService.Application.Interfaces
{
    public interface IGenerateTokenJWT
    {
        TokenResponse HandleGenerateJWT(Guid id, string email, string role);
    }
}
