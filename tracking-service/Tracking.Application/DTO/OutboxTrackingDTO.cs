namespace tracking_service.Tracking.Application.DTO
{
    public class OutboxTrackingDTO
    {
        public Guid OutboxId { get; set; } // Thêm property này

        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public string EventTyep { get; set; } // Typo: Nên sửa thành EventType
        public Guid? ProductId { get; set; }
        public DateTime? Created { get; set; }
    }
}
