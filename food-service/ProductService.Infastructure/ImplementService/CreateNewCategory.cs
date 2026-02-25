using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Aggragate;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Domain.ValueOject;

namespace food_service.ProductService.Infastructure.ImplementService
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
