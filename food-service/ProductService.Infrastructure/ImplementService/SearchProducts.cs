using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Infrastructure.Models;
using System.Net.WebSockets;

namespace food_service.ProductService.Infrastructure.ImplementService
{
    public class SearchProducts : ISearchProducts
    {
        private readonly FoodProductsDbContext _db;
        private readonly IMinIOFood _minio;

        public SearchProducts(FoodProductsDbContext foodProductsDbContext, IMinIOFood minIOFood)
        {
            _db = foodProductsDbContext;
            _minio = minIOFood;
        }

        public async Task<List<ProductDTO>> SearchProductsAsync(string query, int index, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);
           
            int indexpage = (index - 1) * 12;
            var products = _db.Products.Where(s => s.Name.Contains(query)).Skip(indexpage).Take(12).Select(t => new ProductDTO
            {
                Name = t.Name,
                Price = t.Price,
                Id = t.Id.ToString(),
                Decriptions = t.Description,
                quantity = t.ProductDailyInventories.Where(inven => inven.InventoryDate >= today).Select(s => s.RemainingQuantity).FirstOrDefault(),
                ImageFoods = t.ProductImages.Select(i => new ImageFood
                {
                    ImageId = i.Id,
                    UrlImage = i.ImageUrl
                }).ToList(),

            }
            ).ToList();

            var tasks = products.SelectMany(p => p.ImageFoods ?? new List<ImageFood>()).Where(img => !string.IsNullOrEmpty(img.UrlImage))
             .Select(async img =>
             {
                 img.UrlImage = await _minio.GetUrlImage("images", img.UrlImage);
             });

            await Task.WhenAll(tasks);

            if (products.Count == 0)
            {
                return new List<ProductDTO>();
            }
            else
            {
                return  products ;
            }

        }
    }
}
