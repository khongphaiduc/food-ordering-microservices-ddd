using food_service.ProductService.Application.Interface;
using food_service.ProductService.Infrastructure.MasstransitProducerRabbitMQ.MessageContract;
using MassTransit;

namespace food_service.ProductService.Infrastructure.MasstransitProducerRabbitMQ.Consumers
{
    public class RecommendFoodConsumer : IConsumer<RecommendationAI>            
    {
        private readonly IRecommendPersonalFood _recommendationByAI;

        public RecommendFoodConsumer(IRecommendPersonalFood recommendPersonalFood)
        {
            _recommendationByAI = recommendPersonalFood;
        }

        public async Task Consume(ConsumeContext<RecommendationAI> context)
        {

            await _recommendationByAI.Execute(context.Message.IdUser);
        }
    }
}
