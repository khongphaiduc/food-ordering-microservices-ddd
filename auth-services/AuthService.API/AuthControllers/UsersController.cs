using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UserService.API.Protos;

namespace auth_services.AuthService.API.AuthControllers
{
    [EnableRateLimiting("token")]
    [Route("api/auth")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ISignUpUser _iUserSignUp;
        private readonly ICheckLogin _iUserLogIn;
        private readonly IUserLogOut _iUserLogOut;


        public UsersController(ISignUpUser signUpUser, ICheckLogin checkLogin, IUserLogOut userLogOut)
        {
            _iUserSignUp = signUpUser;
            _iUserLogIn = checkLogin;
            _iUserLogOut = userLogOut;

        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(RequestUserLogin user, CancellationToken token = default)
        {
            var result = await _iUserLogIn.IsUserLoginAsync(user, token);

            if (!result.IsLoginSuccessful)
            {
                return Unauthorized(result.Message);
            }

            return Ok(result);
        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout(Guid id, CancellationToken token = default)
        {
            var result = await _iUserLogOut.Execute(id);

            if (result)
            {
                return Ok(new { message = "Logout successful" });
            }
            else
            {
                return BadRequest(new { message = "Logout failed" });
            }
        }




        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(RequestCreateNewUser user, CancellationToken token = default)
        {
            var result = await _iUserSignUp.Execute(user, token);

            if (result)
            {

                return Created(" ", new { message = "Create New User Successful" });
            }
            else
            {
                return BadRequest("The account already exists");
            }

        }


    }
}
