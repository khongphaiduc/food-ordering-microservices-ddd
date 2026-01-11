using auth_services.AuthService.Domain.Entities;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace auth_services.AuthService.Infastructure.Reposistory
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

        // thêm mới refresh token
        public async Task<bool> AddNewRefreshToken(Guid userId, string refreshToken, DateTime expiryDate)
        {
            var userAggregate = await _iUserRepositoty.GetUserById(userId);     // lấy map sang aggregate

            userAggregate.AddReFreshToken(RefreshTokenEntity.CreateNewRefreshToken(refreshToken, expiryDate));   // thêm mới refresh token vào aggregate

            var result = await _iUserRepositoty.UpdateUserRefreshToken(userAggregate);   // cập nhật user

            return result;
        }

        public async Task<bool> IsRevokedToken(string token)
        {
            var tokens = await _db.RefreshTokens.Where(s => s.Token == token && s.RevokedAt != null).FirstOrDefaultAsync();
            return tokens != null ? true : false;
        }

        // thu hồi token
        public async Task<bool> RevokedToken(Guid id)
        {
            var refreshToken = await _db.RefreshTokens.Where(s => s.UserId == id && s.RevokedAt == null).FirstOrDefaultAsync();

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
