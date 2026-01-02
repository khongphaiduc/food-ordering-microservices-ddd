using auth_service.authservice.application.dtos;
using auth_service.authservice.application.InterfaceApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace auth_service.authservice.api.authControlller
{
    [Route("api/users")]
    [ApiController]
    public class RefreshAccessTokenController : ControllerBase
    {
        private readonly IAuthenticationToken _iauthenicatioin;
        private readonly IProvideoAccessToken _iprovideAccessToken;

        public RefreshAccessTokenController(IAuthenticationToken authenticationToken, IProvideoAccessToken provideoAccessToken)
        {
            _iauthenicatioin = authenticationToken;
            _iprovideAccessToken = provideoAccessToken;
        }

        // cấp lại access token
        [HttpPost("accesstoken")]
        [Authorize]
        public async Task<IActionResult> RequestProvideAccessToken([FromBody] RequetsProvideAccessToken request)
        {
            var tokenResult = await _iprovideAccessToken.ProvideAccessToken(request.UserId);
            return Created("", new TokenResult() { TypeToken = tokenResult.TypeToken, Token = tokenResult.Token, TimeCreate = tokenResult.TimeCreate, TimeExpire = tokenResult.TimeExpire });
        }

    }
}
