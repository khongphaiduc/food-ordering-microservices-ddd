using auth_services.AuthService.Domain.Aggregate;

namespace auth_services.AuthService.Domain.Interface
{
    public interface IUserRepository
    {
        Task AddNewUser(UserAggregate userAggregate, CancellationToken token = default);
        Task<bool> UpdateUserRefreshToken(UserAggregate userAggregate, CancellationToken token = default);
        Task<bool> IsExitUser(string email, CancellationToken token = default);
        Task<UserAggregate> GetUserById(Guid id, CancellationToken token = default);
    }
}
