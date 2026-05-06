using auth_services.AuthService.API.CustomExceptions;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
using System.Text.Json;

namespace auth_services.AuthService.Infastructure.ServiceImpelemt
{
    public class CheckLogin : ICheckLogin
    {
        private readonly IHashPassword _IhashPassword;
        private readonly IEnumerable<IGanarateTokenJWT> _iGenarateToken;
        private readonly IRefreshTokenRepository _iRefreshToken;
        private readonly ILogger<CheckLogin> _logger;
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepo;

        public CheckLogin(IUserRepository userRepository, IDistributedCache distributedCache, IHashPassword hashPassword, IEnumerable<IGanarateTokenJWT> ganarateTokenJWTs, IRefreshTokenRepository refreshTokenRepository, ILogger<CheckLogin> logger)
        {
    
            _IhashPassword = hashPassword;
            _iGenarateToken = ganarateTokenJWTs;
            _iRefreshToken = refreshTokenRepository;
            _logger = logger;
            _cache = distributedCache;
            _userRepo = userRepository;
        }

        public async Task<ResponseLoginUser> IsUserLoginAsync(RequestUserLogin user, CancellationToken token = default)
        {

            var realUserInDataBase = await _userRepo.GetUserByEmail(user.Email, token);

            if (realUserInDataBase == null)
            {
                return new ResponseLoginUser()
                {
                    IsLoginSuccessful = false,
                    Message = "User not found"
                };
            }

            var PasswordFromUserSend = _IhashPassword.HandleHashPassword(user.Password, realUserInDataBase.paswordSalt);

            if (PasswordFromUserSend == realUserInDataBase.PasswordHash)
            {

                var sessionID = Guid.NewGuid();

                var option = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                var cacheKey = $"SessionLogin:{realUserInDataBase.Id}";
                await _cache.SetStringAsync(cacheKey, sessionID.ToString(), option, token);

                var access = _iGenarateToken.OfType<GanarateAccessTokenJWT>().First().HandleGenarateJWT(realUserInDataBase.Id, realUserInDataBase.Email, realUserInDataBase.Roles.FirstOrDefault() ?? "Customer");
                var refresh = _iGenarateToken.OfType<GanarateRefresheTokenJWT>().First().HandleGenarateJWT(realUserInDataBase.Id, realUserInDataBase.Email, realUserInDataBase.Roles.FirstOrDefault() ?? "Customer");

                var result = await _iRefreshToken.AddNewRefreshToken(realUserInDataBase.Id, refresh.TokenValue, refresh.ExpireAt);
                _logger.LogInformation($"Create new Refresh Token {result}");
                return new ResponseLoginUser()
                {
                    IsLoginSuccessful = true,
                    Id = realUserInDataBase.Id,
                    Email = realUserInDataBase.Email,
                    AccessToken = access,
                    RefreshToken = refresh,
                    Message = "Login successful",
                    IdSession = sessionID
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
