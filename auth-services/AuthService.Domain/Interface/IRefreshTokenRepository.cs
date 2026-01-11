namespace auth_services.AuthService.Domain.Interface
{
    public interface IRefreshTokenRepository
    {
        Task<bool> AddNewRefreshToken(Guid userId, string refreshToken, DateTime expiryDate);

        Task<bool> RevokedToken(Guid id);

        Task<bool> IsRevokedToken(string token);
    }
}
