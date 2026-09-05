using food_service.ProductService.API.GlobalExceptions;
using food_service.ProductService.Application.DTOs;
using food_service.ProductService.Domain.Aggregate;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Domain.ValueObject;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FoodProductsDbContext _db;
 

        public CategoryRepository(FoodProductsDbContext foodProductsDbContext, ILogger<CategoryRepository> logger)
        {
            _db = foodProductsDbContext;
   
        }

        public async Task<bool> AddNewCategory(CategoryAggregate NewCategoty)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var CategoryModel = new Category
                {
                    Id = NewCategoty.Id,
                    Name = NewCategoty.Name.Value,
                    Description = NewCategoty.Description,
                    IsActive = NewCategoty.IsActive,
                    CreatedAt = NewCategoty.CreateAt,
                    UpdatedAt = NewCategoty.UpdateAt,
                };

               
                await _db.Categories.AddAsync(CategoryModel);
                await _db.SaveChangesAsync();





                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }

        }

        public async Task<ResponseAddCategoryDto> AddNewCategory(RequestCreateCategoryDto request)
        {
            try
            {
                _db.Categories.Add(new Category
                {
                    Name = request.Name,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                });

                var affectrow = await _db.SaveChangesAsync();

                if (affectrow > 0)
                {
                    return new ResponseAddCategoryDto
                    {
                        Status = true,
                        Message = "Add new category successfully"
                    };
                }
                else
                {
                    return new ResponseAddCategoryDto
                    {
                        Status = false,
                        Message = "Add new category failed"
                    };
                }
            }
            catch (Exception ex)
            {
              
                throw;
            }

        }

        public async Task<CategoryAggregate> GetCategoryById(Guid Id)
        {
            var CategoryOrigin = await _db.Categories.Where(s => s.Id == Id).FirstOrDefaultAsync();

            if (CategoryOrigin != null)
            {
                return new CategoryAggregate(CategoryOrigin.Id, new Name(CategoryOrigin.Name), CategoryOrigin.Description, CategoryOrigin.IsActive, CategoryOrigin.CreatedAt, CategoryOrigin.UpdatedAt);
            }
            throw new NotFoundCategoryException($"Not found Category Id : {Id}");
        }

        public async Task<List<ResponseCategoryDto>> GetAllCategory()
        {
            try
            {
                var listCategory = await _db.Categories.Select(c => new ResponseCategoryDto
                {
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

                if (listCategory != null)
                {
                    return listCategory;
                }
                return new List<ResponseCategoryDto>();
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public async Task<bool> UpdateCategory(CategoryAggregate updateCategoty)
        {
            var category = await _db.Categories.Where(s => s.Id == updateCategoty.Id).FirstOrDefaultAsync();

            if (category != null)
            {
                category.Name = updateCategoty.Name.Value;
                category.Description = updateCategoty.Description;
                category.IsActive = updateCategoty.IsActive;

            }
            return await _db.SaveChangesAsync() > 0;
        }

    }
}
