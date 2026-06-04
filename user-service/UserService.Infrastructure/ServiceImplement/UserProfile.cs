using Microsoft.EntityFrameworkCore;
using user_service.userservice.infastructure.DBcontextService;
using user_service.UserService.Application.DTOS;
using user_service.UserService.Application.Services;
using user_service.UserService.Domain.Aggregates;
using user_service.UserService.Domain.Interfaces;
using user_service.UserService.Domain.ValueObjects;

namespace user_service.UserService.Infrastructure.ServiceImplement
{
    public class UserProfile : IUserProfile
    {
        private readonly FoodUsersContext _db;
        private readonly IUserRepository _iUserRepositoty;
        private readonly ILogger<UserProfile> _logger;

        public UserProfile(FoodUsersContext foodUsersContext, IUserRepository userRepository, ILogger<UserProfile> logger)
        {
            _db = foodUsersContext;
            _iUserRepositoty = userRepository;
            _logger = logger;
        }

        public async Task<bool> UserProfilHandle(RequestUserProfile requestUserProfile)
        {
            _logger.LogInformation($"Start create new information user : {requestUserProfile.FullName}");
            var user = await _db.Users.FirstOrDefaultAsync(s => s.Id == requestUserProfile.Id);


            var checkEmail = await _iUserRepositoty.IsEmailExistsAsync(requestUserProfile.Email);

            if (checkEmail)
            {
                return false;
            }

            if (user == null)
            {
                _logger.LogInformation($"Writing information user {requestUserProfile.FullName} is Successful");
                var newUser = UserAggregate.CreateNewUser(requestUserProfile.Id, requestUserProfile.FullName, new Email(requestUserProfile.Email), new PhoneNumber(requestUserProfile.PhoneNumber));
                return await _iUserRepositoty.AddNewUserAsync(newUser);
            }
            else
            {
                _logger.LogWarning($"Writing information user {requestUserProfile.FullName} failure");
                return false;
            }
        }
    }
}
