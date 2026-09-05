using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Infrastructure.DbContextAuth;

namespace auth_services.AuthService.Infrastructure.ServiceImplement
{
    public class SetupDatbase : ISetupDatbase
    {
        private FoodAuthContext _db;

        public SetupDatbase(FoodAuthContext foodAuthContext)
        {
            _db = foodAuthContext;
        }

        public async Task<bool> SetupDatabaseAsync(CancellationToken cancellationToken = default)
        {
            var AdminRole = _db.Roles.FirstOrDefault(r => r.Name == "Admin");

            if (AdminRole == null)
            {
                var role = new Models.Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Description = "Administrator role with full access"
                };
                _db.Roles.Add(role);

            }

            var UserRole = _db.Roles.FirstOrDefault(r => r.Name == "Customer");
            if (UserRole == null)
            {
                var role = new Models.Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer",
                    Description = "Regular user role with limited access"
                };
                _db.Roles.Add(role);

            }

            _db.Users.Add(new Models.User
            {
                Email = "admin@gmail.com",
                PasswordHash = "FoBkN2E/igSuHADha9MHH0jUf4biUsN0ackKrmgAqwg=",
                PasswordSalt = "60fa4f26688840a7adf3601f870ebb52",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Roles = new List<Models.Role> { AdminRole },
                Username = "admin",
                Id = Guid.NewGuid(),
                UpdatedAt = null
            });

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
