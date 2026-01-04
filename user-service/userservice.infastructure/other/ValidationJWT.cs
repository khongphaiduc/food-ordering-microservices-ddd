using System.IdentityModel.Tokens.Jwt;
using user_service.userservice.application.interfaceApplications;

namespace user_service.userservice.infastructure.other
{
    public class ValidationJWT : IValidationJWT
    {
        public string GetTypeToken(HttpContext httpContext)
        {
                
            // Lấy token từ header Authorization
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token not found");

            // Tạo handler để đọc token
            var handler = new JwtSecurityTokenHandler();

            // Parse token thành JwtSecurityToken
            var jwtToken = handler.ReadJwtToken(token);

            // Lấy claim TypeToken
            var typeTokenClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "TypeToken")?.Value;

            if (typeTokenClaim == null)
                throw new Exception("TypeToken claim not found");

            return typeTokenClaim;
        }
    }
}
