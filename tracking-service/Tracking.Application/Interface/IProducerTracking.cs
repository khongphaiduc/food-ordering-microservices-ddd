using tracking_service.Tracking.Application.DTO;

namespace tracking_service.Tracking.Application.Interface
{
    public interface IProducerTracking
    {
        Task SendMessage(TrackingDTO request);
    }
}
