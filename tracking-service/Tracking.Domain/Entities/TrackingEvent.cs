namespace tracking_service.Tracking.Domain.Entities
{
    public class TrackingEvent
    {
        public long Id { get; private set; }
        public Guid? UserId { get; private set; }
        public Guid? SessionId { get; private set; }
        public string EventType { get; private set; }
        public Guid? ProductId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private TrackingEvent() { }

        public TrackingEvent(Guid? userId, Guid? sessionId, string eventType, Guid? productId)
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            UserId = userId;
            SessionId = sessionId;
            EventType = eventType;
            ProductId = productId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
