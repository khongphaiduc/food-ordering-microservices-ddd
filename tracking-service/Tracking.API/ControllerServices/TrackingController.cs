using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using tracking_service.Tracking.API.gRPCimplement;
using tracking_service.Tracking.Application.DTO;
using tracking_service.Tracking.Application.Interface;

namespace tracking_service.Tracking.API.ControllerServices
{
    [Route("api/tracking")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly IUserBehaviorTracking _trackingService;
        private readonly IServiceAI _aiService;
        private readonly IGetBehaviourOfUser _dataUser;
        private readonly LoadFullProductService _recommend;

        public TrackingController(IUserBehaviorTracking userBehaviorTracking, IServiceAI aiService, IGetBehaviourOfUser getBehaviourOfUser, LoadFullProductService loadFullProductService)
        {
            _trackingService = userBehaviorTracking;
            _aiService = aiService;
            _dataUser = getBehaviourOfUser;
            _recommend = loadFullProductService;
        }

        [HttpGet("gemini-chat")]
        public async Task<IActionResult> GeminiChat([FromQuery] string prompt = "Xin chào")
        {

            var result = await _aiService.Prompt(prompt);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> RecordBehaviorUserTracking([FromBody] TrackingDTO request)
        {
            await _trackingService.Execute(request);
            return Ok();
        }



        [HttpGet("{Id}")]
        public async Task<IActionResult> TestData([FromRoute] Guid Id)
        {
            var result = await _dataUser.Execute(Id);
            return Ok(result);
        }



        [HttpGet("recommend/{Id}")]
        public async Task<IActionResult> TestRe([FromRoute] Guid Id)
        {
            await _recommend.ExecutePushDataOnAI(Id);
            return Ok();
        }
    }
}