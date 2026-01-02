using auth_service.authservice.application.dtos;

namespace auth_service.authservice.application.InterfaceApplication
{
    public interface IProvideoAccessToken
    {
        Task<TokenResult> ProvideAccessToken(Guid userId);

    }
}
