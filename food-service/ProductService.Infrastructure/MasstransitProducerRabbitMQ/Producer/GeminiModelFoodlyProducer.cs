using food_service.ProductService.Infrastructure.MasstransitProducerRabbitMQ.MessageContract;
using MassTransit;

namespace food_service.ProductService.Infrastructure.MasstransitProducerRabbitMQ.Producer
{
    public class GeminiModelFoodlyProducer
    {
        private readonly ILogger<GeminiModelFoodlyProducer> _logger;
        private readonly IPublishEndpoint _publishEndPoint;

        public GeminiModelFoodlyProducer(ILogger<GeminiModelFoodlyProducer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndPoint = publishEndpoint;
        }


        public async Task<bool> SendMessage(RecommendationAI reconmendationMessage)
        {
            try
            {

                await _publishEndPoint.Publish(reconmendationMessage);
                _logger.LogInformation("The system sends food recommendations to users. : {UserId}", reconmendationMessage.IdUser);
                return true;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, " L?i khi g?i tin nh?n lên RabbitMQ cho UserId: {UserId}", reconmendationMessage.IdUser);
                return false;
            }
        }



    }
}
