using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace auth_services.AuthService.Infastructure.Security
{
    public class ProvideAccessToken : IProvideAccessToken
    {
        private readonly IEnumerable<IGanarateTokenJWT> _iGenarateToken;
        private readonly IUserRepository _iUserRepository;
        private readonly FoodAuthContext _db;

        public ProvideAccessToken(IEnumerable<IGanarateTokenJWT> ganarateTokenJWTs, IUserRepository userRepository, FoodAuthContext foodAuthContext)
        {
            _iGenarateToken = ganarateTokenJWTs;
            _iUserRepository = userRepository;
            _db = foodAuthContext;
        }

        public async Task<ResponseAccessToken> Handle(RequestProvideAccessToken request)
        {
            var user = await _db.Users.Where(s => s.Id == request.Id && s.Email == request.Email && s.Roles.Any(s => s.Name == request.Role)).FirstOrDefaultAsync();

            if (user == null)
            {
                return new ResponseAccessToken()
                {
                    IsSuccess = false,
               
                };
            }

            var token = _iGenarateToken.OfType<GanarateAccessTokenJWT>().First().HandleGenarateJWT(request.Id, request.Email, request.Role);

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
