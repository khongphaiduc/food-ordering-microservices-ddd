using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Interface;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class UpdateCategory : IUpdateCategory
    {
        private readonly ICategoryRepository _iCategoryRepo;

        public UpdateCategory(ICategoryRepository categoryRepository)
        {
            _iCategoryRepo = categoryRepository;
        }

        public async Task<bool> ExecuteAsync(UpdateCategoryDTO updateCategoryDTO)
        {

            var categoryAggregate = await _iCategoryRepo.GetCategoryById(updateCategoryDTO.Id);

            if (categoryAggregate != null)
            {
                if (!string.IsNullOrEmpty(updateCategoryDTO.Name))
                {
                    categoryAggregate.ChangeName(new Domain.ValueOject.Name(updateCategoryDTO.Name!));
                }

                if (!string.IsNullOrEmpty(updateCategoryDTO.Description))
                {
                    categoryAggregate.ChangeDescription(updateCategoryDTO.Description!);
                }

                if (updateCategoryDTO.IsActive != null)
                {
                    categoryAggregate.ChangeIsActiveOrUnActive(updateCategoryDTO.IsActive.Value);
                }
                return await _iCategoryRepo.UpdateCategory(categoryAggregate);
            }
            else
            {
                return false;
            }

        }
    }
}
