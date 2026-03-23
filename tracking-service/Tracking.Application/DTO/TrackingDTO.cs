using tracking_service.Tracking.Domain.Enums;

namespace tracking_service.Tracking.Application.DTO
{
    public class TrackingDTO
    {
        public Guid IdSession { get; set; }

        public Guid IdUser { get; set; }
        public List<TrackingPayload>? PayLoad { get; set; }
    }


    public class TrackingPayload
    {
        public EventType EventType { get; set; }

        public Guid? IdProduct { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
