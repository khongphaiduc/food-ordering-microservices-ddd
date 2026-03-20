using tracking_service.Tracking.Application.DTO;

namespace tracking_service.Tracking.Domain.Repository
{
    public interface IOutBoxParttern
    {
        Task<List<OutboxTrackingDTO>> GetMessageNotProcessd();

        Task Remaker(Guid IdMessage);
    }
}
