using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.Models;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;

namespace auth_services.AuthService.Infrastructure.ServiceImplement
{
    public class AddStaffRepository : IAddStaffRepository
    {
        private readonly FoodAuthContext _db;
        private readonly ILogger<AddStaffRepository> _logger;

        public AddStaffRepository(FoodAuthContext foodAuthContext, ILogger<AddStaffRepository> logger)
        {
            _db = foodAuthContext;
            _logger = logger;
        }

        public async Task<bool> AddStaffAsync(UserAggregate userAggregate, Guid IDRole)
        {
            var role = await _db.Roles.FirstOrDefaultAsync(s => s.Id == IDRole);
            if (role == null)
            {

                return false;
            }

            var newStaff = new User
            {
                Id = userAggregate.Id,
                Username = userAggregate.Username.Value,
                Email = userAggregate.Email.EmailAdress,
                PasswordHash = userAggregate.PasswordHash,
                PasswordSalt = userAggregate.PasswordSalt,
                IsActive = userAggregate.IsActive,
                CreatedAt = userAggregate.CreatedAt,
                UpdatedAt = userAggregate.UpdatedAt,

                Roles = new List<Role> { role }
            };
            try
            {
                await _db.Users.AddAsync(newStaff);
                var result = await _db.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Bug in file AddStaffRepository.cs" + ex.Message);
                return false;
            }
        }

    }
}
