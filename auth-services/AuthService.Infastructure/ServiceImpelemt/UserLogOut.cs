using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;

namespace auth_services.AuthService.Infastructure.ServiceImpelemt
{
    public class UserLogOut : IUserLogOut
    {
        private readonly IRefreshTokenRepository _iRefreshToken;

        public UserLogOut(IRefreshTokenRepository refreshTokenRepository)
        {
            _iRefreshToken = refreshTokenRepository;
        }

        public Task<bool> Execute(Guid userId)
        {
            var result = _iRefreshToken.RevokedToken(userId);

            return result;
        }
    }
}
