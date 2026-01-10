using auth_services.AuthService.Domain.Aggregate;

namespace auth_services.AuthService.Domain.Interface
{
    public interface IUserRepository
    {
        Task<bool> AddNewUser(UserAggregate userAggregate);

        Task<bool> UpdateUserRefreshToken(UserAggregate userAggregate);
        Task<bool> IsExitUser(string email);

        Task<UserAggregate> GetUserById(Guid id);

    }
}
