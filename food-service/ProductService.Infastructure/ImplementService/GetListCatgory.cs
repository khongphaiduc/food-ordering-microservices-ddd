using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class GetListCatgory : IGetListCatgory
    {
        private readonly FoodProductsDbContext _db;

        public GetListCatgory(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;
        }

        public async Task<List<CatgoryDTO>> Excute()
        {
            var listCategory = await _db.Categories
                .Select(s => new CatgoryDTO
                {
                    IdCategory = s.Id,
                    NameCategory = s.Name,
                })
                .ToListAsync();

            return listCategory;
        }
    }
}
