namespace food_service.ProductService.Application.DTOs
{
    public class ResponseCategoryDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; }
    }
}
