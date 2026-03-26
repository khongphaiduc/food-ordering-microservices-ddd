using food_service.ProductService.Application.Interface;
using food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.MessageContract;
using MassTransit;

namespace food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.Consumers
{
    public class RecommendFoodConsumer : IConsumer<ReconmendationAI>            
    {
        private readonly IRecommenPersonalFood _recommendationByAI;

        public RecommendFoodConsumer(IRecommenPersonalFood recommenPersonalFood)
        {
            _recommendationByAI = recommenPersonalFood;
        }

        public async Task Consume(ConsumeContext<ReconmendationAI> context)
        {

            await _recommendationByAI.Execute(context.Message.IdUser);
        }
    }
}
