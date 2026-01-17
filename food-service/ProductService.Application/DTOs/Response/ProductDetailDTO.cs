namespace food_service.ProductService.Application.DTOs.Response
{
    public class ProductDetailDTO
    {

        public Guid IdProduct { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public List<ProductImageDTO> productImageDTOs { get; set; }

        public List<ProductVariantDTO> productVariantDTOs { get; set; }

    }
}
