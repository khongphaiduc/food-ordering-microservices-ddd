using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;
using UserService.API.Protos;

namespace auth_services.AuthService.API.AuthControllers
{
    [EnableRateLimiting("token")]
    [Authorize(AuthenticationSchemes = "RefreshToken")]
    [Route("api/auth")]
    [ApiController]
    public class ProvideAccessTokenController : ControllerBase
    {
        private readonly IProvideAccessToken _iProvideToken;

        public ProvideAccessTokenController(IProvideAccessToken provideAccessToken)
        {
            _iProvideToken = provideAccessToken;

        }

        [HttpPost("accesstoken")]
        public async Task<IActionResult> AccessToken([FromBody] RequestProvideAccessToken request, CancellationToken tokens = default)
        {
            var token = await _iProvideToken.Handle(request, tokens);
            if (!token.IsSuccess)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "User not exit"
                });
            }
            return Ok(token);
        }
    }
}

