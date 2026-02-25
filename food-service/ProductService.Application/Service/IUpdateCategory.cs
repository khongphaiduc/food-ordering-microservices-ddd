using food_service.ProductService.Application.DTOs.Request;

namespace food_service.ProductService.Application.Service
{
    public interface IUpdateCategory
    {
        Task<bool> ExecuteAsync(UpdateCategoryDTO updateCategoryDTO);
    }
}
