using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Interface
{
    public interface IGetListCategory
    {
        Task<List<CategoryDTO>> Excute();
    }
}
