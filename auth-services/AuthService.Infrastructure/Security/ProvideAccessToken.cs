using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace auth_services.AuthService.Infrastructure.Security
{
    public class ProvideAccessToken : IProvideAccessToken
    {
        private readonly IHttpContextAccessor _httcontext;
        private readonly IEnumerable<IGenerateTokenJWT> _iGenerateToken;
        private readonly IUserRepository _iUserRepository;
        private readonly FoodAuthContext _db;
        private readonly IRefreshTokenRepository _iRefreshTokenRepository;

        public ProvideAccessToken(IEnumerable<IGenerateTokenJWT> generateTokenJWTs, IUserRepository userRepository, FoodAuthContext foodAuthContext, IHttpContextAccessor httpContextAccessor, IRefreshTokenRepository refreshTokenRepository)
        {
            _httcontext = httpContextAccessor;
            _iGenerateToken = generateTokenJWTs;
            _iUserRepository = userRepository;
            _db = foodAuthContext;
            _iRefreshTokenRepository = refreshTokenRepository;
        }

        public async Task<ResponseAccessToken> Handle(RequestProvideAccessToken request, CancellationToken tokens = default)
        {

            var tokenUser = _httcontext.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
            
            var isRevoked = await _iRefreshTokenRepository.IsRevokedToken(tokenUser!); // Check whether the refresh token was invoked 

            if (isRevoked)
            {
                return new ResponseAccessToken()
                {
                    IsSuccess = false,
                    Message = "Token was revoked"
                };
            }

            var user = await _db.Users.Where(s => s.Id == request.Id && s.Email == request.Email && s.Roles.Any(s => s.Name == request.Role)).FirstOrDefaultAsync(tokens);

            if (user == null)
            {
                return new ResponseAccessToken()
                {
                    IsSuccess = false,

                };
            }

            var token = _iGenerateToken.OfType<GenerateAccessTokenJWT>().First().HandleGenerateJWT(request.Id, request.Email, request.Role);

            return new ResponseAccessToken()
            {
                IsSuccess = true,
                TokenType = token.TokenType,
                TokenValue = token.TokenValue,
                CreateAt = token.CreateAt,
                ExpireAt = token.ExpireAt,
                Message = "Access token generado correctamente"
            };

        }

    }
}
