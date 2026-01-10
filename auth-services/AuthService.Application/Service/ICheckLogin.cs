using auth_services.AuthService.Application.DTOS;

namespace auth_services.AuthService.Application.Service
{
    public interface ICheckLogin
    {
        Task<ResponseLoginUser> IsUserLoginAsync(RequestUserLogin user);

    }
}
