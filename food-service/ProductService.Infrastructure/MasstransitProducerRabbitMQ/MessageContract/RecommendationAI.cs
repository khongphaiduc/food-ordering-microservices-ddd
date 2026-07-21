namespace food_service.ProductService.Infrastructure.MasstransitProducerRabbitMQ.MessageContract
{
    public record RecommendationAI      // MassTransit  sử dụng tên của class này để đặt tên Exchange
    {
        public Guid IdUser { get; init; }
    }

}
// MassTransit mặc định sử dụng loại Fanout Exchange  , có nghia là mọi hàng đợi (queue) nào được liên kết với Exchange này, sẽ nhận được tất cả các message từ exchange này.