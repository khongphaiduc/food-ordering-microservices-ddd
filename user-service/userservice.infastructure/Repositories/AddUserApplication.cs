using System.Net.WebSockets;
using user_service.userservice.application.dtos;
using user_service.userservice.application.interfaceApplications;
using user_service.userservice.domain.entity;
using user_service.userservice.domain.interfaces;
using user_service.userservice.domain.value_object;

namespace user_service.userservice.infastructure.Repositories
{
    public class AddUserApplication : IAddUserApplication
    {
        private readonly IUserRepositories _iUserRepo;

        public AddUserApplication(IUserRepositories userRepositories)
        {
            _iUserRepo = userRepositories;
        }

        public async Task<bool> Handle(RequestPersonalInforUsers users)
        {

            var userEntity = new UsersEntity(users.IdUser, users.Name, new Email(users.Email), new PhoneNumber(users.PhoneNumber));

            return await _iUserRepo.AddUser(userEntity);

        }
    }
}
