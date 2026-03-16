using System;
using System.Collections.Generic;

namespace tracking_service.Tracking.Infastructrure.Models;

public partial class TrackingEvent
{
    public long Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? SessionId { get; set; }

    public string? EventType { get; set; }

    public Guid? ProductId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual UserSession? Session { get; set; }
}
