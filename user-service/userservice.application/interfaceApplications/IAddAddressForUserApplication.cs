using user_service.userservice.application.dtos;

namespace user_service.userservice.application.interfaceApplications
{
    public interface IAddAddressForUserApplication
    {
        Task<bool> Handle(RequestInfoAddressUser addressUser);

    }
}
