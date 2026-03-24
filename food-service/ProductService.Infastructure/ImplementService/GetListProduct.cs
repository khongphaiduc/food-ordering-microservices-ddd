using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Minio;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class GetListProduct : IGetListProduct
    {
        private readonly FoodProductsDbContext _db;
        private readonly IDistributedCache _redisCatch;
        private readonly IConfiguration _config;
        private readonly IMinIOFood _minio;
        private readonly IDistributedCache _cache;

        public GetListProduct(IDistributedCache distributedCache, FoodProductsDbContext foodProductsDbContext, IDistributedCache _RedisCache, IConfiguration config, IMinIOFood minioClient)
        {
            _db = foodProductsDbContext;
            _redisCatch = _RedisCache;
            _config = config;
            _minio = minioClient;
            _cache = distributedCache;
        }

        // search và phân trang
        public async Task<List<ProductDTO>> ExecuteAsync(RequestGetListProduct request)
        {

            var numberSkip = (request.PageIndex - 1) * request.PageSize;

            var query = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(p => p.Name.Contains(request.Keyword));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            query = query
                .OrderBy(p => p.Id)
                .Skip(numberSkip)
                .Take(request.PageSize);

            var listProduct = await query
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

            var tasks = listProduct.SelectMany(p => p.ImageFoods ?? new List<ImageFood>()).Where(img => !string.IsNullOrEmpty(img.UrlImage))
              .Select(async img =>
              {
                  img.UrlImage = await _minio.GetUrlImage("images", img.UrlImage);
              });

            await Task.WhenAll(tasks);

            return listProduct;
        }

        public async Task<int> TotalProdut()
        {
            return await _db.Products.CountAsync();
        }


        //var listAllProduct = await _db.Products.Select(s => new { s.Id, s.Name }).ToListAsync();

        //var content = JsonSerializer.Serialize(listAllProduct);

        //await _cache.SetStringAsync("ALLPRODUCT", content, new DistributedCacheEntryOptions
        //    {
        //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        //    });
    }
}
