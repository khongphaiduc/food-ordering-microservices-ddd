using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Interface
{
    public interface IRecommendPersonalFood
    {
        Task Execute(Guid IdUser);

        Task<List<ProductDTO>> Execute1(Guid IdUser);
    }
}
