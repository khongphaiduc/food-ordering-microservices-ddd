using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infrastructure.ImplementService
{
    public class GetListCategory : IGetListCategory
    {
        private readonly FoodProductsDbContext _db;

        public GetListCategory(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;
        }

        public async Task<List<CategoryDTO>> Excute()
        {
            var listCategory = await _db.Categories
                .Select(s => new CategoryDTO
                {
                    IdCategory = s.Id,
                    NameCategory = s.Name,
                })
                .ToListAsync();

            return listCategory;
        }
    }
}
