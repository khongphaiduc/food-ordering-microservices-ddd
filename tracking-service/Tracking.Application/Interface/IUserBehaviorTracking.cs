using tracking_service.Tracking.Application.DTO;

namespace tracking_service.Tracking.Application.Interface
{
    public interface IUserBehaviorTracking
    {
        Task Execute(TrackingDTO request);
    }
}
