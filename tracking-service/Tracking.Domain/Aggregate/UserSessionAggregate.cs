using tracking_service.Tracking.Domain.Entities;

namespace tracking_service.Tracking.Domain.Aggregate
{
    public class UserSessionAggregate
    {

        public Guid Id { get; private set; }
        public Guid? UserId { get; private set; }
        public DateTime StartedAt { get; private set; }

        private readonly List<TrackingEvent> _events = new();
        public IReadOnlyCollection<TrackingEvent> Events => _events.AsReadOnly();

        private UserSessionAggregate() { }

        public static UserSessionAggregate CreateNewSessoionUser(Guid? userId)
        {
            return new UserSessionAggregate
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartedAt = DateTime.UtcNow
            };
        }

        public void AddEvent(string eventType, Guid? productId)
        {
            var trackingEvent = new TrackingEvent(UserId, Id, eventType, productId);
            _events.Add(trackingEvent);
        }

    }
}
