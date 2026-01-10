using auth_services.AuthService.Application.DTOS;

namespace auth_services.AuthService.Application.Interfaces
{
    public interface IProvideAccessToken
    {
       Task< ResponseAccessToken> Handle(RequestProvideAccessToken request);
    }
}
