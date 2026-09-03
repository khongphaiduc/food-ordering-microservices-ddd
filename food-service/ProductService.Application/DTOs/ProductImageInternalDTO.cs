namespace food_service.ProductService.Application.DTOs
{
    public class ProductImageInternalDTO
    {
        public Guid Id { get; set; }

        public string URLImage { get; set; }

        public bool IsMain { get; set; }
    }
}
