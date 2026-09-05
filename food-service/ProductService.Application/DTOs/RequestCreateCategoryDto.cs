using MassTransit;

namespace food_service.ProductService.Application.DTOs
{
    public class RequestCreateCategoryDto
    {
         public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = false;

    }
}
