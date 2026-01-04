using user_service.userservice.domain.entity;

namespace user_service.userservice.domain.interfaces
{
    public interface IUserRepositories
    {

        Task<bool> AddUser(UsersEntity user);

        Task<bool> UpdateUser(UsersEntity user);

        Task<bool> AddAdressForUser(AddressUserEntity addressUser);

        Task<bool> UpdateAdressForUser(AddressUserEntity addressUser);

    }
}
