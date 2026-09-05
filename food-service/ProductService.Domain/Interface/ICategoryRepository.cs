using food_service.ProductService.Application.DTOs;
using food_service.ProductService.Domain.Aggregate;

namespace food_service.ProductService.Domain.Interface
{
    public interface ICategoryRepository
    {
        Task<bool> AddNewCategory(CategoryAggregate NewCategoty);

        Task<ResponseAddCategoryDto> AddNewCategory(RequestCreateCategoryDto request);

        Task<CategoryAggregate> GetCategoryById(Guid Id);
        Task<bool> UpdateCategory(CategoryAggregate UpdateCategoty);

        Task<List<ResponseCategoryDto>> GetAllCategory();
    }
}
