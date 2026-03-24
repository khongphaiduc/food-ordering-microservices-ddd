using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Interface
{
    public interface IRecommenPersonalFood
    {
        Task<List<ProductDTO>> Execute(Guid IdUser);
    }
}
