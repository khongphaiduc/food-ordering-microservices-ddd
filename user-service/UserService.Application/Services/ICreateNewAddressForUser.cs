using user_service.UserService.Application.DTOS;

namespace user_service.UserService.Application.Services
{
    public interface ICreateNewAddressForUser
    {
        Task Execute(RequestCreateNewAddressUser request);
    }
}
