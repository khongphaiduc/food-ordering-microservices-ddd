using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infrastructure.ImplementService
{
    public class ProductRecommendationService : IProductRecommendationService
    {
        private readonly FoodProductsDbContext _db;
        private readonly IR2Food _r2Food;

        public ProductRecommendationService(FoodProductsDbContext db, IR2Food r2Food )
        {
            _db = db;
            _r2Food = r2Food;
        }

        public async Task<List<ProductDTO>> ExecuteAsync(Guid IdCategory)
        {

            var listProductRecommendation = await _db.Products
                .Where(p => p.CategoryId != IdCategory)
                .OrderBy(p => Guid.NewGuid())
                .Take(8)
                .Select(p => new ProductDTO
                {
                    IdCategory = p.CategoryId,
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Price = p.Price,
                    ImageFoods = p.ProductImages.Select(s => new ImageFood { ImageId = s.Id, IsMain = s.IsMain, UrlImage = s.ImageUrl }).ToList(),
                    IsAvailable = p.IsAvailable,
                    Decriptions = p.Description
                })
                .ToListAsync();
            var tasks = listProductRecommendation.SelectMany(p => p.ImageFoods ?? new List<ImageFood>()).Where(img => !string.IsNullOrEmpty(img.UrlImage))
             .Select(async img =>
             {
                 img.UrlImage = await _r2Food.GetUrlImage("images", img.UrlImage);
             });

            await Task.WhenAll(tasks);
            return listProductRecommendation;
        }
    }
}
