namespace auth_services.AuthService.Domain.Entities
{
    public class RefreshTokenEntity
    {
        public Guid Id { get; private set; }

        public string Token { get; private set; }

        public DateTime ExpireAt { get; private set; }

        public DateTime CreateAt { get; private set; }

        public DateTime Revoked { get; private set; }

        public string Device { get; private set; }
        private RefreshTokenEntity()
        {
        }

        internal RefreshTokenEntity(Guid id, string token, DateTime expireAt, DateTime createAt, string device)
        {
            Id = id;
            Token = token;
            ExpireAt = expireAt;
            CreateAt = createAt;
            Device = device;
        }

        public static RefreshTokenEntity CreateNewRefreshToken(string Token, DateTime ExpireAt)
        {
            return new RefreshTokenEntity()
            {
                Id = Guid.NewGuid(),
                Token = Token,
                ExpireAt = ExpireAt,
                CreateAt = DateTime.UtcNow,
                Device = "Website"
            };
        }

    }
}
