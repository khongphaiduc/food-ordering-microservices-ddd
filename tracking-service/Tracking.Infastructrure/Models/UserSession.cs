using System;
using System.Collections.Generic;

namespace tracking_service.Tracking.Infastructrure.Models;

public partial class UserSession
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? StartedAt { get; set; }

    public virtual ICollection<TrackingEvent> TrackingEvents { get; set; } = new List<TrackingEvent>();
}
