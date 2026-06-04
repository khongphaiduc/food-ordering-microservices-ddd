using notification_service.Notification.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace notification_service.Models;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid Userid { get; set; }

    public string Recipient { get; set; } = null!;
    public string Content { get; set; } = null!;

    public string? Providerresponse { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

}
