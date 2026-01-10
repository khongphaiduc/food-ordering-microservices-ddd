
using auth_services.AuthService.Domain.Entities;
using auth_services.AuthService.Domain.ValueObject;
namespace auth_services.AuthService.Domain.Aggregate
{
    public class UserAggregate
    {
        public Guid Id { get; set; }

        public FullNameOfUser Username { get; private set; }

        public Email Email { get; private set; }

        public string PasswordHash { get; private set; }

        public string PasswordSalt { get; private set; }


        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }


        private List<RefreshTokenEntity> _reFreshToken = new List<RefreshTokenEntity>();

        public IReadOnlyList<RefreshTokenEntity> ReFreshToken => _reFreshToken.AsReadOnly();
      

        private UserAggregate()
        {
        }

        internal UserAggregate(Guid id, FullNameOfUser username, Email email, string passwordHash, string passwordSalt, bool isActive, DateTime createdAt, DateTime? updatedAt, List<RefreshTokenEntity> reFreshToken)
        {
            Id = id;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            _reFreshToken = reFreshToken;
        }

        public void AddReFreshToken(RefreshTokenEntity refreshToken)
        {
            _reFreshToken.Add(refreshToken);
        }

        public static UserAggregate CreateNewUser(FullNameOfUser username, Email email, string passwordHash, string passwordSalt)
        {
            return new UserAggregate()
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

    }
}
