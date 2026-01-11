using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace auth_services.AuthService.API.AuthControllers
{
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
        public async Task<IActionResult> AccessToken([FromBody] RequestProvideAccessToken request)
        { 
            var token = await _iProvideToken.Handle(request);
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
