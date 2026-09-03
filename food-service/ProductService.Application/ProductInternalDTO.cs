using food_service.ProductService.Application.DTOs;

namespace food_service.ProductService.Application
{
    public class ProductInternalDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
        public string Description { get; set; }

        public Guid IdCategory { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public List<ProductImageInternalDTO> productImageInternalDTOs { get; set; }

        public List<ProductVariantInternalDTO> productVariantInternalDTOs { get; set; }

    }
}
