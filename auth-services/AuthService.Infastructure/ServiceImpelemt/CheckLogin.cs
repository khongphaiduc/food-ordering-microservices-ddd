using auth_services.AuthService.API.CustomExceptions;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace auth_services.AuthService.Infastructure.ServiceImpelemt
{
    public class CheckLogin : ICheckLogin
    {
        private readonly FoodAuthContext _db;
        private readonly IHashPassword _IhashPassword;
        private readonly IEnumerable<IGanarateTokenJWT> _iGenarateToken;
        private readonly IRefreshTokenRepository _iRefreshToken;
        private readonly ILogger<CheckLogin> _logger;

        public CheckLogin(FoodAuthContext foodAuthContext, IHashPassword hashPassword, IEnumerable<IGanarateTokenJWT> ganarateTokenJWTs, IRefreshTokenRepository refreshTokenRepository, ILogger<CheckLogin> logger)
        {
            _db = foodAuthContext;
            _IhashPassword = hashPassword;
            _iGenarateToken = ganarateTokenJWTs;
            _iRefreshToken = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<ResponseLoginUser> IsUserLoginAsync(RequestUserLogin user)
        {
            var realUserInDataBase = await _db.Users.Where(s => s.Email == user.Email).Select(s => new
            {
                passwordHash = s.PasswordHash,
                paswordSalt = s.PasswordSalt,
                id = s.Id,
                s.Email,
                Role = s.Roles.Select(s => s.Name).FirstOrDefault() ?? "Customer",
            }).FirstOrDefaultAsync();

            if (realUserInDataBase == null)
            {
                return new ResponseLoginUser()
                {
                    IsLoginSuccessful = false,
                    Message = "User not found"
                };
            }

            var s = _IhashPassword.HandleHashPassword(user.Password, realUserInDataBase.paswordSalt);

            if (s == realUserInDataBase.passwordHash)
            {

                var access = _iGenarateToken.OfType<GanarateAccessTokenJWT>().First().HandleGenarateJWT(realUserInDataBase.id, realUserInDataBase.Email, realUserInDataBase.Role);
                var refresh = _iGenarateToken.OfType<GanarateRefresheTokenJWT>().First().HandleGenarateJWT(realUserInDataBase.id, realUserInDataBase.Email, realUserInDataBase.Role);

                var result = await _iRefreshToken.AddNewRefreshToken(realUserInDataBase.id, refresh.TokenValue, refresh.ExpireAt);
                _logger.LogInformation($"Create new Refresh Token {result}");
                return new ResponseLoginUser()
                {
                    IsLoginSuccessful = true,
                    Id = realUserInDataBase.id,
                    AccessToken = access,
                    RefreshToken = refresh,
                    Message = "Login successful",

                };
            }
            else
            {
                return new ResponseLoginUser()
                {
                    IsLoginSuccessful = false,
                    Message = "Password is incorrect"
                };
            }
        }
    }
}
