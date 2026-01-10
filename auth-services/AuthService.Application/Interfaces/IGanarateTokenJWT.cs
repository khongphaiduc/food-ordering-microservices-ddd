using auth_services.AuthService.Application.DTOS;

namespace auth_services.AuthService.Application.Interfaces
{
    public interface IGanarateTokenJWT
    {
        TokenResponse HandleGenarateJWT(Guid id, string email, string role);
    }
}
