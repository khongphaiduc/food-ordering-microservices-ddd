using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tracking_service.Tracking.Application.DTO;
using tracking_service.Tracking.Application.Interface;

namespace tracking_service.Tracking.API.ControllerServices
{
    [Route("api/tracking")]
    [Authorize(AuthenticationSchemes ="AccessToken")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        public IUserBehaviorTracking tracking_service { get; }
        public TrackingController(IUserBehaviorTracking userBehaviorTracking)
        {
            tracking_service = userBehaviorTracking;
        }

        [HttpPost]
        public async Task<IActionResult> RecordBehaviorUserTracking([FromBody] TrackingDTO request)
        {
            await tracking_service.Execute(request);
            return Ok();
        }


    }
}
