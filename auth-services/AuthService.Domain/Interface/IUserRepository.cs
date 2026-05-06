using auth_services.AuthService.Domain.Aggregate;

namespace auth_services.AuthService.Domain.Interface
{
    public interface IUserRepository
    {
        Task AddNewUser(UserAggregate userAggregate, CancellationToken token = default);
        Task<bool> UpdateUserRefreshToken(UserAggregate userAggregate, CancellationToken token = default);
        Task<bool> IsExitUser(string email, CancellationToken token = default);
        Task<UserAggregate> GetUserById(Guid id, CancellationToken token = default);

        Task<UserInformation> GetUserByEmail(string email, CancellationToken token = default);

    }



    public class UserInformation
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string paswordSalt { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
