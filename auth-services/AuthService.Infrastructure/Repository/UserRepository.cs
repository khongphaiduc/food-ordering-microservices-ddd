using auth_services.AuthService.API.CustomExceptions;
using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Entities;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Domain.ValueObject;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace auth_services.AuthService.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly FoodAuthContext _db;

        public UserRepository(FoodAuthContext foodAuthContext)
        {
            _db = foodAuthContext;
        }

        // thêm user 
        public async Task AddNewUser(UserAggregate userAggregate, CancellationToken token = default)
        {
            var role = await _db.Roles.FirstOrDefaultAsync(s => s.Name == "Customer", token);

            if (role == null)
            {
                role = new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer",
                    Description = "Role for customer",
                };
                _db.Roles.Add(role);
                await _db.SaveChangesAsync(token);
            }

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

        }

        // l?y user r?i map sang  Aggregate
        public async Task<UserAggregate> GetUserById(Guid id, CancellationToken token = default)
        {
            var userAggregate = await _db.Users.Include(s => s.RefreshTokens).Where(s => s.Id == id).FirstOrDefaultAsync(token);

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

        public async Task<UserInformation> GetUserByEmail(string email, CancellationToken token = default)
        {
            var user = await _db.Users.Include(s => s.Roles).Where(s => s.Email == email).Select(s => new UserInformation()
            {
                Id = s.Id,
                PasswordHash = s.PasswordHash,
                paswordSalt = s.PasswordSalt,
                Username = s.Username,
                Email = s.Email,
                Roles = s.Roles.Select(s => s.Name).ToList()
            }).FirstOrDefaultAsync(token);

            if (user == null)
            {
                throw new NotfoundExceptions("Not found user");
            }
            return user;
        }

        public Task<bool> IsExitUser(string email, CancellationToken token = default)
        {
            return _db.Users.AnyAsync(u => u.Email == email, token);
        }


        // c?p nh?t user ph?n refresh token
        public async Task<bool> UpdateUserRefreshToken(UserAggregate userAggregate, CancellationToken token = default)
        {

            var user = await _db.Users
           .Include(s => s.RefreshTokens)
           .FirstOrDefaultAsync(s => s.Id == userAggregate.Id, token);

            if (user == null) return false;

            user.Username = userAggregate.Username.Value;
            user.Email = userAggregate.Email.EmailAdress;
            user.PasswordHash = userAggregate.PasswordHash;
            user.PasswordSalt = userAggregate.PasswordSalt;
            user.IsActive = userAggregate.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // revoke token cu
            var oldToken = user.RefreshTokens.FirstOrDefault(s => s.RevokedAt == null);
            if (oldToken != null)
            {
                oldToken.RevokedAt = DateTime.Now;
            }

            // thêm m?i token , b? qua token dã có
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
