namespace auth_services.AuthService.Domain.Interface
{
    public interface IRefreshTokenRepository
    {
        Task<bool> AddNewRefreshToken(Guid userId, string refreshToken, DateTime expiryDate, CancellationToken token = default);

        Task<bool> RevokedToken(Guid id, CancellationToken token = default);

        Task<bool> IsRevokedToken(string token, CancellationToken tokens = default);
    }
}
