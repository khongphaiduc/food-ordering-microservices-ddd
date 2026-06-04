using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace auth_services.AuthService.Infrastructure.Tokens
{
    public class GenerateAccessTokenJWT : IGenerateTokenJWT
    {
        private readonly IConfiguration _iconfig;

        public GenerateAccessTokenJWT()
        {
        }

        public GenerateAccessTokenJWT(IConfiguration configuration)
        {
            _iconfig = configuration;
        }
        // access token
        public virtual TokenResponse HandleGenerateJWT(Guid id, string email, string role)
        {
            var time = int.Parse(_iconfig["Jwt:Time:AccessToken"]!);

            var listClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,id.ToString()),
                new Claim(ClaimTypes.Name,email),
                new Claim (ClaimTypes.Role,role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfig["Jwt:Key:AccessToken"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _iconfig["Jwt:Issuer"],
                audience: _iconfig["Jwt:Audience"],
                claims: listClaim,
                expires: DateTime.UtcNow.AddHours(time),
                signingCredentials: creds);


            return new TokenResponse()
            {
                TokenType = "AccessToken",
                TokenValue = new JwtSecurityTokenHandler().WriteToken(token),
                CreateAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddHours(time)
            };
        }
    }
}
