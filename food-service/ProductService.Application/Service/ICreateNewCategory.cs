using food_service.ProductService.Application.DTOs.Request;

namespace food_service.ProductService.Application.Service
{
    public interface ICreateNewCategory
    {
        Task<bool> ExecuteAsync(CreateNewCategoryDTO createNewCategoryDTO);
    }
}
