using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using tracking_service.Tracking.Application.DTO;
using tracking_service.Tracking.Domain.Repository;
using tracking_service.Tracking.Infastructrure.Models;

namespace tracking_service.Tracking.Infastructrure.ImplementServices
{
    public class OutBoxParttern : IOutBoxParttern
    {
        private readonly FoodProductsDbContext _db;
        private readonly ILogger<OutBoxParttern> _logger;

        public OutBoxParttern(FoodProductsDbContext foodProductsDbContext, ILogger<OutBoxParttern> logger)
        {
            _db = foodProductsDbContext;
            _logger = logger;
        }

        public async Task<List<OutboxTrackingDTO>> GetMessageNotProcessd()
        {

            var pendingMessages = await _db.OutBoxPatterns
                .Where(m => m.Status == 0)
                .OrderBy(m => m.CreateAt)
                .ToListAsync();

            var result = new List<OutboxTrackingDTO>();

            foreach (var msg in pendingMessages)
            {
                if (string.IsNullOrWhiteSpace(msg.Payload)) continue;

                try
                {
                    // Giải mã Payload thành DTO
                    var dto = JsonSerializer.Deserialize<OutboxTrackingDTO>(msg.Payload, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (dto != null)
                    {
                        // Gán Id của Outbox vào DTO để sau khi xử lý xong sẽ dùng gọi hàm Remaker
                        dto.OutboxId = msg.Id;
                        result.Add(dto);
                    }
                }
                catch (JsonException s)
                {
                    _logger.LogError($"Error :{s.Message}");
                }
            }

            return result;
        }

        public async Task Remaker(Guid IdMessage)
        {
            var message = await _db.OutBoxPatterns.FindAsync(IdMessage);
            if (message != null)
            {

                message.Status = 1;
                message.ProcessedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
    }
}