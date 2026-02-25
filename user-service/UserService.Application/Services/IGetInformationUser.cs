using user_service.UserService.Application.DTOS;

namespace user_service.UserService.Application.Services
{
    public interface IGetInformationUser
    {
        Task<InformationUserDTO> Execute(Guid IdUser);
    }
}
