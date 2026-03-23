using Microsoft.AspNetCore.Mvc;
using tracking_service.Tracking.Application.DTO;
using tracking_service.Tracking.Application.Interface;

namespace tracking_service.Tracking.API.ControllerServices
{
    [Route("api/tracking")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly IUserBehaviorTracking _trackingService;
        private readonly IServiceAI _aiService; // Inject Service AI vào đây

        public TrackingController(IUserBehaviorTracking userBehaviorTracking, IServiceAI aiService)
        {
            _trackingService = userBehaviorTracking;
            _aiService = aiService;
        }

        [HttpGet("gemini-chat")]
        public async Task<IActionResult> GeminiChat([FromQuery] string prompt = "Xin chào")
        {
            // Gọi qua service đã tách
            var result = await _aiService.Prompt(prompt);
            return Ok(result);
        }

        // --- GIỮ NGUYÊN CÁC HÀM CŨ ---
        [HttpPost]
        public async Task<IActionResult> RecordBehaviorUserTracking([FromBody] TrackingDTO request)
        {
            await _trackingService.Execute(request);
            return Ok();
        }

        [HttpGet]
        public IActionResult racking()
        {
            return Ok();
        }

        [HttpGet("test-api")]
        public IActionResult TestAPI()
        {
            var content = "Xin chào tôi là Đức";
            return Ok(content);
        }
    }
}