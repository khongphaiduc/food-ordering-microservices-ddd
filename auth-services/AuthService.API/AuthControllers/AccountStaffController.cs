using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace auth_services.AuthService.API.AuthControllers
{
    //[EnableRateLimiting()]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    [Route("api/auth")]
    [ApiController]
    public class AccountStaffController : ControllerBase
    {
        private readonly IAddAccountStaffs _addStaff;
        private readonly IGetListStaff _listStaff;

        public AccountStaffController(IAddAccountStaffs accountStaffs, IGetListStaff listStaff)
        {
            _addStaff = accountStaffs;
            _listStaff = listStaff;
        }


        [HttpPost("admin/staff")]
        public async Task<ActionResult> AddStaff([FromBody] AddAccountStaffDTO request, CancellationToken token = default)
        {
            var result = await _addStaff.AddAccountStaffsAsync(request, token);
            if (!result) { return BadRequest(new { status = "Add Staff Failed or Staff already exists" }); }

            return Ok(new { status = "Add Staff Successful" });
        }

        [HttpGet("admin/staff")]
        public async Task<IActionResult> ViewListStaff(CancellationToken token = default)
        {
            var list = await _listStaff.Execute(token);
            return Ok(list);
        }


        [HttpGet("admin/roles")]
        public async Task<IActionResult> ViewListRole(CancellationToken token = default)
        {
            var list = await _listStaff.GetListRole(token);
            return Ok(list);
        }


    }
}
