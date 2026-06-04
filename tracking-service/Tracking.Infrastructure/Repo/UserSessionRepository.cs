using Microsoft.EntityFrameworkCore;
using tracking_service.Tracking.Domain.Aggregate;
using tracking_service.Tracking.Domain.Repository;
using tracking_service.Tracking.Infrastructure.Models;

namespace tracking_service.Tracking.Infrastructure.Repo
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly FoodProductsDbContext _db;

        public UserSessionRepository(FoodProductsDbContext context)
        {
            _db = context;
        }

        public async Task<bool> SessionExists(Guid sessionId)
        {
            return await _db.UserSessions
                .AnyAsync(x => x.Id == sessionId);
        }

        public async Task<bool> AddNewUserSession(UserSessionAggregate sessionAggregate)
        {
            var entity = new UserSession
            {
                Id = sessionAggregate.Id,
                UserId = sessionAggregate.UserId,
                StartedAt = sessionAggregate.StartedAt,
                TrackingEvents = sessionAggregate.Events.Select(e => new TrackingEvent
                {
                    UserId = e.UserId,
                    SessionId = e.SessionId,
                    EventType = e.EventType,
                    ProductId = e.ProductId,
                    CreatedAt = e.CreatedAt
                }).ToList()
            };

            _db.UserSessions.Add(entity);

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddEvent(Domain.Entities.TrackingEvent trackingEvent)
        {
            var entity = new TrackingEvent
            {
                UserId = trackingEvent.UserId,
                SessionId = trackingEvent.SessionId,
                EventType = trackingEvent.EventType,
                ProductId = trackingEvent.ProductId,
                CreatedAt = trackingEvent.CreatedAt
            };

            _db.TrackingEvents.Add(entity);

            return await _db.SaveChangesAsync() > 0;
        }

    }
}
