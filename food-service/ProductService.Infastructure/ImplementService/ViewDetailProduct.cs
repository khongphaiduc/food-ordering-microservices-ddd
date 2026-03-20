using food_service.ProductService.API.GlobalExceptions;
using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class ViewDetailProduct : IViewDetailProduct
    {
        private readonly FoodProductsDbContext _db;
        private readonly IDistributedCache _redis;
        private readonly IMinIOFood _minio;
        private readonly IConfiguration _configuration;

        public ViewDetailProduct(FoodProductsDbContext foodProductsDbContext, IDistributedCache distributedCache, IMinIOFood minIOFood, IConfiguration configuration)
        {
            _db = foodProductsDbContext;
            _redis = distributedCache;
            _minio = minIOFood;
            _configuration = configuration;
        }

        public async Task<ProductDetailDTO?> ExecuteAsync(Guid idProduct)
        {
            var cache = await _redis.GetStringAsync(idProduct.ToString());

            if (cache != null)
            {
                return JsonSerializer.Deserialize<ProductDetailDTO>(cache);
            }

            var product = await _db.Products
                .Where(s => s.Id == idProduct)
                .Select(s => new ProductDetailDTO
                {
                    IdCategory = s.CategoryId,
                    IdProduct = s.Id,
                    Name  = s.Name,
                    Description = s.Description ?? "None",
                    Price = s.Price,
                    productImageDTOs = s.ProductImages
                        .Select(c => new ProductImageDTO
                        {
                            IdImage = c.Id,
                            UrlImage = c.ImageUrl,
                            IsMain = c.IsMain,
                        }).ToList(),
                    productVariantDTOs = s.ProductVariants
                        .Select(g => new ProductVariantDTO
                        {
                            IdVariant = g.Id.ToString(),
                            Name = g.Name,
                            ExtraPrice = g.ExtraPrice,
                            TypeProduct = "Variant"
                        }).ToList(),
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return null;

            var tasks = product.productImageDTOs
                .Where(i => !string.IsNullOrEmpty(i.UrlImage))
                .Select(async i =>
                {
                    i.UrlImage = await _minio.GetUrlImage("images", i.UrlImage);      
                });

            await Task.WhenAll(tasks);

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(6));

            await _redis.SetStringAsync(
                idProduct.ToString(),
                JsonSerializer.Serialize(product),
                options);

            return product;
        }

    }
}
