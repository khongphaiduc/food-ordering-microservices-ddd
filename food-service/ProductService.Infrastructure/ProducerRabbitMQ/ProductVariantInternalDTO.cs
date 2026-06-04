namespace food_service.ProductService.Infrastructure.ProducerRabbitMQ
{
    public class ProductVariantInternalDTO
    {
        public Guid IdProduct { get; set; }

        public string Name { get; set; }

        public decimal  Extra_Price { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;  

        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
