
using auth_service.authservice.application.dtos;
using auth_service.authservice.application.InterfaceApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth_service.authservice.api.authControlller
{
    [Route("api/auth")]
    [ApiController]
    public class UserAuthenticationController : ControllerBase
    {
        private readonly ICreateUserHandler _createUerHandler;
        private readonly IUserLogin _iUserLogin;
        private readonly IAuthenticationToken _iAuthen;
        private readonly IRefreshTokensRepositories _iRefreshToken;

        public UserAuthenticationController(ICreateUserHandler createUserHandler, IUserLogin userLogin, IAuthenticationToken authenticationToken, IRefreshTokensRepositories refreshTokensRepositories)
        {
            _createUerHandler = createUserHandler;
            _iUserLogin = userLogin;
            _iAuthen = authenticationToken;
            _iRefreshToken = refreshTokensRepositories;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> AuthenticateUser([FromBody] RequestAccount registerAccount)
        {
            var result = await _createUerHandler.HandleCreateUser(registerAccount);
            if (result.IsSuccessful == false)
            {
                return BadRequest(new { Success = result.IsSuccessful, Message = result.Message });
            }
            return Created("", new { Success = result.IsSuccessful, Message = result.Message });
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] RequestAccount loginAccount)
        {
            var result = await _iUserLogin.LoginHandler(loginAccount);

            if (result.IsSuccess)
            {
                var infoToken = _iAuthen.GenerateToken(loginAccount.Email, "Customer", "AccessToken"); // access token

                await _iRefreshToken.RevokeOldToken(result.UserId.Value); // thu hoi token cu

                var refreshTokenInfor = await _iRefreshToken.AddRefreshToken(result.UserId.Value); // refresh token

                return Ok(new { Id = result.UserId, account = loginAccount.Email, accessToken = infoToken, refreshToken = refreshTokenInfor });

            }
            else
            {
                return Unauthorized(new { status = result.IsSuccess, email = loginAccount.Email, message = result.Message });
            }

        }

        [Authorize]
        [HttpPut("logout")]
        public async Task<IActionResult> UserLogOut([FromBody] LogOutRequest request)
        {

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var resultCheckToken = await _iRefreshToken.IsInvokedToken(token);

            if (resultCheckToken) return Unauthorized(new { message = "Token has been revoked" });

            await _iRefreshToken.RevokeOldToken(request.UserId);

            return Ok(new { message = "Logout successfully"});
        }

    }
}
