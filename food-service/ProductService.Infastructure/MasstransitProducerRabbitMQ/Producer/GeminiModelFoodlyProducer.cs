using food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.MessageContract;
using MassTransit;

namespace food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.Producer
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


        public async Task<bool> SendMessage(ReconmendationAI reconmendationMessage)
        {
            try
            {

                await _publishEndPoint.Publish(reconmendationMessage);
                _logger.LogInformation(" Đã gửi yêu cầu gợi ý món ăn cho UserId: {UserId}", reconmendationMessage.IdUser);
                return true;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, " Lỗi khi gửi tin nhắn lên RabbitMQ cho UserId: {UserId}", reconmendationMessage.IdUser);
                return false;
            }
        }



    }
}
