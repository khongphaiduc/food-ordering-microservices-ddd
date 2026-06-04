using Microsoft.EntityFrameworkCore;
using user_service.userservice.infastructure.DBcontextService;
using user_service.UserService.Application.DTOS;
using user_service.UserService.Application.Services;
using user_service.UserService.Domain.Aggregates;
using user_service.UserService.Domain.Entities;
using user_service.UserService.Domain.Interfaces;

namespace user_service.UserService.Infrastructure.ServiceImplement
{
    public class CreateNewAddressForUser : ICreateNewAddressForUser
    {
        private readonly FoodUsersContext _db;
        private readonly IUserRepository _userRepo;

        public CreateNewAddressForUser(FoodUsersContext foodUsers, IUserRepository userRepository)
        {
            _db = foodUsers;
            _userRepo = userRepository;
        }

        public async Task Execute(RequestCreateNewAddressUser request)
        {
            var user = await _db.Users.Include(s => s.UserAddresses).FirstOrDefaultAsync(s => s.Id == request.IdUser);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var listAddress = user.UserAddresses.Select(s => new UserAddresses(s.Id, s.UserId, s.AddressLine1, s.AddressLine2, s.City, s.District, s.PostalCode, s.IsDefault, s.CreatedAt, s.UpdatedAt)).ToList();
            var userAggregate = new UserAggregate(user.Id, user.FullName, new Domain.ValueObjects.Email(user.Email), new Domain.ValueObjects.PhoneNumber(user.PhoneNumber), user.IsActive, user.CreatedAt, user.CreatedAt, listAddress);

            userAggregate.AddNewAddress(UserAddresses.CreateNewAddress(request.IdUser, request.Line1, request.Line2, request.City, request.District, "OOO", request.IsDefault));
            await _userRepo.UpdateUserAsync(userAggregate);

        }
    }
}
