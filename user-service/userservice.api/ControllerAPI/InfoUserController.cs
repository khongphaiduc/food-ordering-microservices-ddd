using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using user_service.userservice.application.dtos;
using user_service.userservice.application.interfaceApplications;

namespace user_service.userservice.api.ControllerAPI
{
    [Route("api/users")]
    [ApiController]
    public class InfoUserController : ControllerBase
    {
        private readonly IAddUserApplication _IAddUserApplication;
        private readonly IAddAddressForUserApplication _IaddAddressUserApplication;
        private readonly IValidationJWT _iValidationJWT;

        public InfoUserController(IAddUserApplication addUserApplication, IAddAddressForUserApplication addAddress, IValidationJWT validationJWT)
        {
            _IAddUserApplication = addUserApplication;
            _IaddAddressUserApplication = addAddress;
            _iValidationJWT = validationJWT;
        }

        [Authorize]
        [HttpPost("person")]
        public async Task<IActionResult> AddUser([FromBody] RequestPersonalInforUsers requestUser)
        {
            var TypeToken = _iValidationJWT.GetTypeToken(this.HttpContext);

            if (TypeToken != "AccessToken") return Unauthorized(new { status = "Token is not valid" });

            var result = await _IAddUserApplication.Handle(requestUser);
            return Created("", new { status = "Add User successfuly" });
        }

        [Authorize]
        [HttpPost("info")]
        public async Task<IActionResult> AddAdressUser([FromBody] RequestInfoAddressUser requestInfo)
        {

            var TypeToken = _iValidationJWT.GetTypeToken(this.HttpContext);

            if (TypeToken != "AccessToken") return Unauthorized(new { status = "Token is not valid" });

            var resutl = await _IaddAddressUserApplication.Handle(requestInfo);

            return Created("", new { status = "Add Address successfuly" });
        }

    }
}
