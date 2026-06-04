using auth_services.AuthService.Domain.Entities;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace auth_services.AuthService.Infrastructure.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly FoodAuthContext _db;
        private readonly IUserRepository _iUserRepositoty;

        public RefreshTokenRepository(FoodAuthContext foodAuthContext, IUserRepository userRepository)
        {
            _db = foodAuthContext;
            _iUserRepositoty = userRepository;
        }

        // thêm m?i refresh token
        public async Task<bool> AddNewRefreshToken(Guid userId, string refreshToken, DateTime expiryDate, CancellationToken token = default)
        {
            var userAggregate = await _iUserRepositoty.GetUserById(userId,token);     // l?y map sang aggregate

            userAggregate.AddReFreshToken(RefreshTokenEntity.CreateNewRefreshToken(refreshToken, expiryDate));   // thêm m?i refresh token vào aggregate

            var result = await _iUserRepositoty.UpdateUserRefreshToken(userAggregate);   

            return result;
        }

        public async Task<bool> IsRevokedToken(string token, CancellationToken cancellationToken = default)
        {
            var tokens = await _db.RefreshTokens.Where(s => s.Token == token && s.RevokedAt != null).FirstOrDefaultAsync(cancellationToken);
            return tokens != null ? true : false;
        }

        // thu h?i token
        public async Task<bool> RevokedToken(Guid id, CancellationToken token = default)
        {
            var refreshToken = await _db.RefreshTokens.Where(s => s.UserId == id && s.RevokedAt == null).FirstOrDefaultAsync(token);

            if (refreshToken != null)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
               
                return await _db.SaveChangesAsync()>0;
            }
            else
            {
                return false;
            }
        }
    }
}
