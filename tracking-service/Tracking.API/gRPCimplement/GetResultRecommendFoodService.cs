using Grpc.Core;
using trackingtService.API.Protos;

namespace tracking_service.Tracking.API.gRPCimplement
{
    public class GetResultRecommendFoodService : GeminiFoodlyGrpc.GeminiFoodlyGrpcBase
    {
        private readonly LoadFullProductService _GeminiAIFood;

        public GetResultRecommendFoodService(LoadFullProductService loadFullProductService)
        {
            _GeminiAIFood = loadFullProductService;
        }

        public override async Task<none> SetListFoodRecommend(RequestRecommendUser request, ServerCallContext context)
        {
            await _GeminiAIFood.ExecutePushDataOnAI(Guid.Parse(request.IdUser));
            return new none();
        }
    }
}
