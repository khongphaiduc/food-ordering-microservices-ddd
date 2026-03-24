using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using productService.API.Protos;
using tracking_service.Tracking.Application.Interface;

namespace tracking_service.Tracking.API.gRPCimplement
{
    public class MappedBehaviourDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string EventType { get; set; }
        public int CountTimes { get; set; }
    }

    public class LoadFullProductService
    {
        private readonly ProductListGrpc.ProductListGrpcClient _loadFullProduct;
        private readonly IGetBehaviourOfUser _dataOfuser;
        private readonly IServiceAI _AIGemini;
        private readonly IDistributedCache _cache;
        private readonly ILogger<LoadFullProductService> _logger;

        public LoadFullProductService(
            ProductListGrpc.ProductListGrpcClient productListGrpcClient,
            IGetBehaviourOfUser getBehaviourOfUser,
            IServiceAI serviceAI,
            IDistributedCache distributedCache,
            ILogger<LoadFullProductService> logger)
        {
            _loadFullProduct = productListGrpcClient;
            _dataOfuser = getBehaviourOfUser;
            _AIGemini = serviceAI;
            _cache = distributedCache;
            _logger = logger;
        }

        public async Task ExecutePushDataOnAI(Guid IdUser)
        {
            try
            {
                _logger.LogInformation("--- Bắt đầu quy trình gợi ý cho User: {UserId} ---", IdUser);

                // 1. Load dữ liệu từ gRPC và Database
                var listProduct = await _loadFullProduct.GetListProductsFoodAsync(new none());
                if (listProduct?.Payload == null || !listProduct.Payload.Any())
                {
                    _logger.LogError("LỖI gRPC: Không lấy được danh sách sản phẩm hoặc danh sách trống.");
                    return;
                }

                var behaviours = await _dataOfuser.Execute(IdUser);
                var productDict = listProduct.Payload.ToDictionary(p => p.IdProduct.ToLower(), p => p.NameProduct);

                // 2. Map hành vi người dùng
                var mappedBehaviours = behaviours.BehaviourUsers
                    .Where(b => productDict.ContainsKey(b.IdProduct.ToString().ToLower()))
                    .Select(b => new MappedBehaviourDto
                    {
                        ProductId = b.IdProduct.ToString().ToLower(),
                        ProductName = productDict[b.IdProduct.ToString().ToLower()],
                        EventType = b.eventType,
                        CountTimes = b.CountTimes
                    })
                    .ToList();

                _logger.LogInformation("User {UserId} có {Count} hành vi hợp lệ.", IdUser, mappedBehaviours.Count);

                // 3. Xử lý trường hợp không có hành vi (Fallback sớm)
                if (!mappedBehaviours.Any())
                {
                    _logger.LogWarning("User {UserId} không có hành vi. Thực hiện lưu 10 món ngẫu nhiên vào Cache.", IdUser);
                    var randomList = FallbackProducts(listProduct.Payload, 10);
                    await SaveToCache(IdUser, randomList);
                    return;
                }

                // 4. Gọi AI Gemini
                var prompt = GeneratePrompt(listProduct.Payload, mappedBehaviours);
                string aiResponse = string.Empty;

                try
                {
                    aiResponse = await _AIGemini.Prompt(prompt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LỖI AI: Không thể kết nối hoặc lỗi khi gọi Gemini cho User {UserId}.", IdUser);
                }

                // 5. Parse dữ liệu từ AI
                var aiIds = ParseAiResponse(aiResponse);

                // 6. Tổng hợp kết quả (Finalize)
                var final = aiIds.Distinct().ToList();



                _logger.LogInformation("AI trả về {Count} sản phẩm cho User {UserId}.", final.Count, IdUser);
                // Nếu AI thiếu, bù đắp từ hành vi (Logic chấm điểm)
                if (final.Count < 10)
                {
                    _logger.LogWarning("AI trả về thiếu ({Count}/10). Đang bù đắp từ hành vi cũ.", final.Count);
                    var behaviorFallback = BuildFallbackFromBehaviour(mappedBehaviours);
                    foreach (var id in behaviorFallback)
                    {
                        if (!final.Contains(id)) final.Add(id);
                        if (final.Count >= 10) break;
                    }
                }

                // Nếu vẫn thiếu, lấy ngẫu nhiên
                if (final.Count < 10)
                {
                    _logger.LogWarning("Vẫn thiếu dữ liệu. Đang bù đắp ngẫu nhiên.");
                    var randomFallback = listProduct.Payload.Select(p => p.IdProduct.ToLower()).OrderBy(_ => Guid.NewGuid());
                    foreach (var id in randomFallback)
                    {
                        if (!final.Contains(id)) final.Add(id);
                        if (final.Count >= 10) break;
                    }
                }

                // 7. Lưu vào Cache
                await SaveToCache(IdUser, final.Take(10).ToList());
                _logger.LogInformation("HOÀN TẤT: Đã cập nhật Cache cho User {UserId}.", IdUser);

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "LỖI NGHIÊM TRỌNG trong ExecutePushDataOnAI của User {UserId}", IdUser);
            }
        }

        private string GeneratePrompt(IEnumerable<Product> products, List<MappedBehaviourDto> behaviours)
        {
            return $@"
            You are a food recommendation system.
            Return ONLY a valid JSON array of product IDs.
            STRICT RULES:
            - Exactly 10 items
            - No explanation, no markdown backticks, just the array.
            Product list (Top 50): {JsonSerializer.Serialize(products.Take(50))}
            User behavior: {JsonSerializer.Serialize(behaviours)}";
        }

        private List<string> ParseAiResponse(string result)
        {
            if (string.IsNullOrWhiteSpace(result)) return new List<string>();

            try
            {
                // Làm sạch chuỗi JSON (Xử lý trường hợp AI trả về text kèm JSON)
                var start = result.IndexOf('[');
                var end = result.LastIndexOf(']');
                if (start >= 0 && end > start)
                {
                    var cleanJson = result.Substring(start, end - start + 1);
                    return JsonSerializer.Deserialize<List<string>>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("LỖI PARSE JSON từ AI: {Message}. Raw Data: {Raw}", ex.Message, result);
            }
            return new List<string>();
        }

        private List<string> BuildFallbackFromBehaviour(List<MappedBehaviourDto> behaviours)
        {
            return behaviours
                .GroupBy(b => b.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Score = g.Sum(x => (x.EventType == "AddToCart" ? 3 : 1) * x.CountTimes)
                })
                .OrderByDescending(x => x.Score)
                .Select(x => x.ProductId)
                .ToList();
        }

        private List<string> FallbackProducts(IEnumerable<Product> products, int count)
        {
            return products
                .Select(p => p.IdProduct.ToLower())
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToList();
        }

        private async Task SaveToCache(Guid userId, List<string> data)
        {
            try
            {
                var cacheKey = $"{userId}ListPersonalIDFood";
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8) };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data), options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LỖI CACHE: Không thể lưu dữ liệu vào Redis cho User {UserId}", userId);
            }
        }
    }
}