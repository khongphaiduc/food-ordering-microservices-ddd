using auth_services.AuthService.Application.DTOS;

namespace auth_services.AuthService.Application.Service
{
    public interface ISignUpUser
    {
        Task<ResponseRegisterAccountDto> Execute(RequestCreateNewUser user, CancellationToken token = default);

    }
}
