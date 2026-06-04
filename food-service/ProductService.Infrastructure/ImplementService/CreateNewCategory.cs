using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Aggregate;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Domain.ValueObject;

namespace food_service.ProductService.Infrastructure.ImplementService
{
    public class CreateNewCategory : ICreateNewCategory
    {
        private readonly ICategoryRepository _iCategoryRepo;

        public CreateNewCategory(ICategoryRepository categoryRepository)
        {
            _iCategoryRepo = categoryRepository;
        }

        public async Task<bool> ExecuteAsync(CreateNewCategoryDTO createNewCategoryDTO)
        {
            var categoryAggregate = CategoryAggregate.CreateNewCategory(new Name(createNewCategoryDTO.Name), createNewCategoryDTO.Description);
            return await _iCategoryRepo.AddNewCategory(categoryAggregate);
        }
    }
}
