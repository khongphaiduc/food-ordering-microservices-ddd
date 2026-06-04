using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using trackingtService.API.Protos;

namespace food_service.ProductService.Infrastructure.ImplementService
{
    public class RecommendPersonalFood : IRecommendPersonalFood
    {
        private readonly FoodProductsDbContext _db;
        private readonly IDistributedCache _cache;
        private readonly GeminiFoodlyGrpc.GeminiFoodlyGrpcClient _GetListProductRecommendByAI;
        private readonly IMinIOFood _minio;
        private readonly ILogger<RecommendPersonalFood> _logger;

        public RecommendPersonalFood(ILogger<RecommendPersonalFood> logger, FoodProductsDbContext foodProductsDbContext, IDistributedCache distributedCache, GeminiFoodlyGrpc.GeminiFoodlyGrpcClient geminiFoodlyGrpcClient, IMinIOFood minio)
        {
            _db = foodProductsDbContext;
            _cache = distributedCache;
            _GetListProductRecommendByAI = geminiFoodlyGrpcClient;
            _minio = minio;
            _logger = logger;
        }

        public async Task Execute(Guid IdUser)
        {
            string KeyPersonal = IdUser.ToString() + "PersonalFoods";      // chua danh sách các món an du?c truy v?n s?n 

            await _GetListProductRecommendByAI.SetListFoodRecommendAsync(new RequestRecommendUser { IdUser = IdUser.ToString() });  // g?i AI d? l?y danh sách các món an du?c g?i ý cho user dó

            var IdProducts = await _cache.GetStringAsync(IdUser.ToString() + "ListPersonalIDFood");  // ListProduct was recommend by AI

            var content = JsonSerializer.Deserialize<List<string>>(IdProducts) ?? new List<string>();

            // các món  du?c g?i ý 
            var recommendedProducts = _db.Products
                .Where(s => content.Contains(s.Id.ToString()))
                .Select(t => new ProductDTO
                {
                    Id = t.Id.ToString(),
                    Name = t.Name,
                    Price = t.Price,
                    Decriptions = t.Description,
                    IdCategory = t.CategoryId,
                    ImageFoods = t.ProductImages.Select(i => new ImageFood
                    {
                        ImageId = i.Id,
                        UrlImage = i.ImageUrl,
                        IsMain = i.IsMain
                    }).ToList(),
                    IsAvailable = t.IsAvailable
                }).ToList();


            await _cache.SetStringAsync(KeyPersonal, JsonSerializer.Serialize(recommendedProducts),
                      new DistributedCacheEntryOptions
                      {
                          AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
                      });

        }

        public async Task<List<ProductDTO>> Execute1(Guid IdUser)
        {
            string dtoCacheKey = IdUser.ToString() + "ListPersonalIDFood";


            string KeyPersonal = IdUser.ToString() + "PersonalFoods";

            var cachedDTOs = await _cache.GetStringAsync(KeyPersonal);

            if (!string.IsNullOrEmpty(cachedDTOs))
            {
                var ListProduct = JsonSerializer.Deserialize<List<ProductDTO>>(cachedDTOs)
                                  ?? new List<ProductDTO>();

                var tasks = ListProduct
                    .SelectMany(s => s.ImageFoods ?? new List<ImageFood>())
                    .Where(s => !string.IsNullOrEmpty(s.UrlImage))
                    .Select(async s =>
                    {
                        s.UrlImage = await _minio.GetUrlImage("images", s.UrlImage);
                    })
                    .ToList();

                await Task.WhenAll(tasks);

                _logger.LogInformation("Cache hit - return products");

                return ListProduct;
            }
            else
            {
                return null;
            }
        }
    }
}
