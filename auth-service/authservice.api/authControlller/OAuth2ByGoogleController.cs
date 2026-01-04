using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using auth_service.authservice.application.InterfaceApplication;
using auth_service.authservice.application.dtos;
using auth_service.authservice.infastructure.Models;

namespace auth_service.authservice.api.authControlller
{
    [Route("api/auth")]
    [ApiController]
    public class OAuth2ByGoogleController : ControllerBase
    {
        private readonly IAuthenticationByGoogle _IauthebyGoogle;
        private readonly IAuthenticationToken _iAuthenCreateToken;
        private readonly IRefreshTokensRepositories _IRefreshTokensRepositories;

        public OAuth2ByGoogleController(IAuthenticationByGoogle byGoogle, IAuthenticationToken authenticationToken, IRefreshTokensRepositories refreshTokensRepositories)
        {
            _IauthebyGoogle = byGoogle;
            _iAuthenCreateToken = authenticationToken;
            _IRefreshTokensRepositories = refreshTokensRepositories;
        }

        [HttpGet("googlelogin")]
        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = "https://localhost:7150/users/google-response"
            });
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("ExternalCookie");

            if (!result.Succeeded || result.Principal == null)     // case user login fail hoặc null
            {
                return Unauthorized();
            }

            // lấy thông tin user từ Google trả về
            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });


            //  kiểm tra xem  đã có user trong db chưa, nếu chưa thì tạo mới
            var user = await _IauthebyGoogle.RegisterUserThroughAuthenticationByGoogle(new ResponseAuthenticationByGoogle()
            {
                Name = result.Principal.FindFirst(c => c.Type == "name")?.Value,
                Email = result.Principal.FindFirst(c => c.Type == "email")?.Value
            });

            // Access Token
            var accessTokens = _iAuthenCreateToken.GenerateToken(user.Email!, "Customer", "AccessToken");

            // Thu hồi refresh token cũ nếu có
            await _IRefreshTokensRepositories.RevokeOldToken(user.Id!.Value);

            // Tạo mới refresh token

            var refreshTokenInfo = await _IRefreshTokensRepositories.AddRefreshToken(user.Id!.Value);


            await HttpContext.SignOutAsync("ExternalCookie");

            return Ok(new { id = user.Id  , account = user.Email , accessToken = accessTokens, refreshToken = refreshTokenInfo });
        }


        [HttpGet("google-error")]
        public IActionResult GoogleError([FromQuery] string message)
        {
            return BadRequest(new
            {
                status = "Error",
                message = "OAuth Correlation Failed",
                detail = message,
                solution = "Hãy xóa Cookie trình duyệt cho localhost và thử lại bằng Tab ẩn danh."
            });
        }
    }
}
