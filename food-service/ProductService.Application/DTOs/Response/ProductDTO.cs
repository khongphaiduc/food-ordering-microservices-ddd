namespace food_service.ProductService.Application.DTOs.Response
{
    public class ProductDTO
    {
        public Guid IdCategory { get; set; }

        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string? Decriptions { get; set; }

        public List<ImageFood>? ImageFoods { get; set; }
        public bool IsAvailable { get; set; }   // còn bán hay không
    }


    public class ImageFood
    {
        public Guid ImageId { get; set; }

        public string UrlImage { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false;
    }
}
