namespace food_service.ProductService.Infrastructure.MasstransitProducerRabbitMQ.MessageContract
{
    public record RecommendationAI      // MassTransit s? s? d?ng tên c?a clas này làm tên c?a Exchange
    {
        public Guid IdUser { get; init; }
    }

}
// MassTransit m?c d?nh s? s? d?ng lo?i Fanout Exchange  , có nghia là m?i hàng d?i (queue) nào du?c liên k?t v?i Exchange này s? nh?n du?c t?t c? các tin nh?n du?c g?i d?n Exchange.