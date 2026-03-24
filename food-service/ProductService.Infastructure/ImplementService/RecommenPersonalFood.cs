using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using trackingtService.API.Protos;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class RecommenPersonalFood : IRecommenPersonalFood
    {
        private readonly FoodProductsDbContext _db;
        private readonly IDistributedCache _cache;
        private readonly GeminiFoodlyGrpc.GeminiFoodlyGrpcClient _GetListProductRecommendByAI;

        public RecommenPersonalFood(FoodProductsDbContext foodProductsDbContext, IDistributedCache distributedCache, GeminiFoodlyGrpc.GeminiFoodlyGrpcClient geminiFoodlyGrpcClient)
        {
            _db = foodProductsDbContext;
            _cache = distributedCache;
            _GetListProductRecommendByAI = geminiFoodlyGrpcClient;
        }

        public async Task<List<ProductDTO>> Execute(Guid IdUser)
        {
            string dtoCacheKey = IdUser.ToString() + "ListPersonalIDFood"; // Key : chưa các id product được AI gợi ý cho user đó


            string KeyPersonal = IdUser.ToString() + "PersonalFoods";      // chưa danh sách các món ăn được truy vẫn sẫn 

            var cachedDTOs = await _cache.GetStringAsync(KeyPersonal);
            if (!string.IsNullOrEmpty(cachedDTOs))   // nếu có cache thì trả về luôn, không cần gọi AI nữa
            {
                return JsonSerializer.Deserialize<List<ProductDTO>>(cachedDTOs) ?? new List<ProductDTO>();
            }
            else
            {

                await _GetListProductRecommendByAI.SetListFoodRecommendAsync(new RequestRecommendUser { IdUser = IdUser.ToString() });  // gọi AI để lấy danh sách các món ăn được gợi ý cho user đó

                var IdProducts = await _cache.GetStringAsync(IdUser.ToString() + "ListPersonalIDFood");  // lấy danh sách các món đượce gợi ý bởi AI

                if (IdProducts == null) return new List<ProductDTO>();  // flag

                var content = JsonSerializer.Deserialize<List<string>>(IdProducts) ?? new List<string>();


                // các món  được gọi ý 
                var inforProductRecommend = _db.Products.Where(s => content.Contains(s.Id.ToString())).Select(t => new ProductDTO
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


                await _cache.SetStringAsync(KeyPersonal, JsonSerializer.Serialize(inforProductRecommend),
                          new DistributedCacheEntryOptions
                          {
                              AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
                          });
                return inforProductRecommend;

            }
        }
    }
}
