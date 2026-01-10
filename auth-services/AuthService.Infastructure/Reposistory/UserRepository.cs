using auth_services.AuthService.API.CustomExceptions;
using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Entities;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Domain.ValueObject;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace auth_services.AuthService.Infastructure.Reposistory
{
    public class UserRepository : IUserRepository
    {
        private readonly FoodAuthContext _db;

        public UserRepository(FoodAuthContext foodAuthContext)
        {
            _db = foodAuthContext;
        }

        // thêm user 
        public async Task<bool> AddNewUser(UserAggregate userAggregate)
        {
            var role = await _db.Roles.FirstOrDefaultAsync(s => s.Name == "Customer") ?? throw new NotfoundExceptions("Not found role User");

            // từ UserAggregate chuyển thành User model để lưu vào database
            var users = new User()
            {
                Id = userAggregate.Id,
                Username = userAggregate.Username.Value,
                Email = userAggregate.Email.EmailAdress,
                PasswordHash = userAggregate.PasswordHash,
                PasswordSalt = userAggregate.PasswordSalt,
                IsActive = userAggregate.IsActive,
                CreatedAt = userAggregate.CreatedAt,
                UpdatedAt = userAggregate.UpdatedAt,
            };

            users.Roles.Add(role);

            await _db.Users.AddAsync(users);
            return await _db.SaveChangesAsync() > 0;
        }

        // lấy user rồi map sang  Aggregate
        public async Task<UserAggregate> GetUserById(Guid id)
        {
            var userAggregate = await _db.Users.Include(s => s.RefreshTokens).Where(s => s.Id == id).FirstOrDefaultAsync();

            if (userAggregate != null)
            {
                return new UserAggregate(
                    userAggregate.Id,
                    new FullNameOfUser(userAggregate.Username),
                    new Email(userAggregate.Email),
                    userAggregate.PasswordHash,
                    userAggregate.PasswordSalt,
                    userAggregate.IsActive,
                    userAggregate.CreatedAt,
                    userAggregate.UpdatedAt,
                    userAggregate.RefreshTokens.Select(s => new RefreshTokenEntity(
                        s.Id,
                        s.Token,
                        s.ExpiresAt,
                        s.CreatedAt,
                        s.Device
                        )).ToList()
                   );

            }
            else
            {
                throw new NotfoundExceptions("Not found user");
            }
        }

        public Task<bool> IsExitUser(string email)
        {
            return _db.Users.AnyAsync(u => u.Email == email);
        }


        // cập nhật user phần refresh token
        public async Task<bool> UpdateUserRefreshToken(UserAggregate userAggregate)
        {

            var user = _db.Users.Include(s => s.RefreshTokens).Where(s => s.Id == userAggregate.Id).FirstOrDefault();

            if (user == null) return false;

            user.Username = userAggregate.Username.Value;
            user.Email = userAggregate.Email.EmailAdress;
            user.PasswordHash = userAggregate.PasswordHash;
            user.PasswordSalt = userAggregate.PasswordSalt;
            user.IsActive = userAggregate.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // revoke token cũ
            var oldToken = user.RefreshTokens.FirstOrDefault(s => s.RevokedAt == null);
            if (oldToken != null)
            {
                oldToken.RevokedAt = DateTime.Now;
            }

            // thêm mới token , bỏ qua token đã có
            foreach (var item in userAggregate.ReFreshToken)
            {
                if (!user.RefreshTokens.Any(s => s.Id == item.Id))
                {
                    user.RefreshTokens.Add(new RefreshToken()
                    {
                        Id = item.Id,
                        UserId = user.Id,
                        Token = item.Token,
                        ExpiresAt = item.ExpireAt,
                        CreatedAt = item.CreateAt,
                        Device = item.Device
                    });
                }
            }
            return await _db.SaveChangesAsync() > 0;
        }

    }
}
