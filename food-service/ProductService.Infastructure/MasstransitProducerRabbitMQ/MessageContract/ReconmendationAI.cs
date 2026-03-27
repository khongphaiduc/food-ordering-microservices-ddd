namespace food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.MessageContract
{
    public record ReconmendationAI      // MassTransit sẽ sử dụng tên của clas này làm tên của Exchange
    {
        public Guid IdUser { get; init; }
    }

}
// MassTransit mặc định sẽ sử dụng loại Fanout Exchange  , có nghĩa là mọi hàng đợi (queue) nào được liên kết với Exchange này sẽ nhận được tất cả các tin nhắn được gửi đến Exchange.