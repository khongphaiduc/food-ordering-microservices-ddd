using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class ProductRecommendationService : IProductRecommendationService
    {
        private readonly FoodProductsDbContext _db;
        private readonly IMinIOFood _minio;

        public ProductRecommendationService(FoodProductsDbContext db, IMinIOFood minIOFood )
        {
            _db = db;
            _minio = minIOFood;
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
                 img.UrlImage = await _minio.GetUrlImage("images", img.UrlImage);
             });

            await Task.WhenAll(tasks);
            return listProductRecommendation;
        }
    }
}
