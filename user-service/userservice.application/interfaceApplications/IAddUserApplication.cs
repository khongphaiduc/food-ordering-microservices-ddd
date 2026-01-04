using user_service.userservice.application.dtos;

namespace user_service.userservice.application.interfaceApplications
{
    public interface IAddUserApplication
    {
        Task<bool> Handle(RequestPersonalInforUsers users);

    }
}
