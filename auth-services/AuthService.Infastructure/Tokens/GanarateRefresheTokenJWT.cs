using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace auth_services.AuthService.Infastructure.Tokens
{
    public class GanarateRefresheTokenJWT : IGanarateTokenJWT
    {
        private readonly IConfiguration _iconfig;
        private readonly ILogger<GanarateRefresheTokenJWT> _ilogger;

        public GanarateRefresheTokenJWT(IConfiguration configuration, ILogger<GanarateRefresheTokenJWT> logger)
        {
            _iconfig = configuration;
            _ilogger = logger;
        }

        // refreshe token
        public TokenResponse HandleGenarateJWT(Guid id, string email, string role)
        {
            var time = int.Parse(_iconfig["Jwt:Time:RefreshToken"]!);

            var listClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfig["Jwt:Key:RefreshToken"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _iconfig["Jwt:Issuer"],
                audience: _iconfig["Jwt:Audience"],
                claims: listClaim,
                expires: DateTime.UtcNow.AddDays(time),
                signingCredentials: creds
                );

            return new TokenResponse()
            {
                TokenType = "RefresheToken",
                TokenValue = new JwtSecurityTokenHandler().WriteToken(token),
                CreateAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddDays(time)
            };
        }
    }
}
