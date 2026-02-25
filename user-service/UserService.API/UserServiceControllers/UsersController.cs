using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using user_service.UserService.Application.DTOS;
using user_service.UserService.Application.Services;

namespace user_service.UserService.API.UserServiceControllers
{
    [Route("api/users")]
    //[Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICreateNewAddressForUser _addAddress;
        private readonly IUserProfile _iUserCreateProfileService;
        private readonly IGetInformationUser _infor;

        public UsersController(IUserProfile userProfile, IGetInformationUser getInformationUser, ICreateNewAddressForUser createNewAddressForUser)
        {
            _addAddress = createNewAddressForUser;
            _iUserCreateProfileService = userProfile;
            _infor = getInformationUser;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser(RequestUserProfile requestUserProfile)
        {
            var result = await _iUserCreateProfileService.UserProfilHandle(requestUserProfile);

            return Ok(result);
        }

        [HttpGet("{IdUser}")]
        public async Task<IActionResult> GetInformationUser([FromRoute] Guid IdUser)
        {
            var infor = await _infor.Execute(IdUser);
            return Ok(infor);
        }


        [HttpPost("address")]
        public async Task<IActionResult> AddAddress([FromBody] RequestCreateNewAddressUser request)
        {
            await _addAddress.Execute(request);
            return Ok();
        }

    }
}
