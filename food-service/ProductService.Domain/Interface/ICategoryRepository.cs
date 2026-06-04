using food_service.ProductService.Domain.Aggregate;

namespace food_service.ProductService.Domain.Interface
{
    public interface ICategoryRepository
    {
        Task<bool> AddNewCategory(CategoryAggregate NewCategoty);

        Task<CategoryAggregate> GetCategoryById(Guid Id);
        Task<bool> UpdateCategory(CategoryAggregate UpdateCategoty);
    }
}
