namespace search_service.SearchService.Infrastructure.ConsumerRabbitMQ
{
    public class ProductImageInternalDTO
    {
        public Guid Id { get; set; }

        public string URLImage { get; set; }

        public bool IsMain { get; set; }
    }
}
