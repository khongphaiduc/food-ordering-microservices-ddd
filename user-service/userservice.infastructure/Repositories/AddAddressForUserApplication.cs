using user_service.userservice.application.dtos;
using user_service.userservice.application.interfaceApplications;
using user_service.userservice.domain.entity;
using user_service.userservice.domain.interfaces;
using user_service.userservice.domain.value_object;

namespace user_service.userservice.infastructure.Repositories
{
    public class AddAddressForUserApplication : IAddAddressForUserApplication
    {
        private readonly IUserRepositories _IAddressUser;

        public AddAddressForUserApplication(IUserRepositories userRepositories)
        {
            _IAddressUser = userRepositories;
        }

        public async Task<bool> Handle(RequestInfoAddressUser addressUser)
        {
            var AddressEntity = new AddressUserEntity()
            {
                UserId = addressUser.IdUser,
                AddressLine1 = new StreetAddress(addressUser.AddressLine1),
                AddressLine2 = addressUser.AddressLine2,
                City = new CityName(addressUser.City),
                PostalCode = new PostalCode(addressUser.PostalCode),
                IsDefault = false,
                CreatedAt = DateTime.UtcNow,
            };

            return await _IAddressUser.AddAdressForUser(AddressEntity);
        }
    }
}
