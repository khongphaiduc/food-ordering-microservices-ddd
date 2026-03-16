using tracking_service.Tracking.Domain.Aggregate;
using tracking_service.Tracking.Domain.Entities;

namespace tracking_service.Tracking.Domain.Repository
{
    public interface IUserSessionRepository
    {
        Task<bool> AddNewUserSession(UserSessionAggregate sessionAggregate);


        Task<bool> AddEvent(TrackingEvent trackingEvent);

        Task<bool> SessionExists(Guid sessionId);

    }
}
