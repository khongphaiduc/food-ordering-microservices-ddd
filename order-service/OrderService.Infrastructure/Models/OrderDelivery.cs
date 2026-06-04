using System;
using System.Collections.Generic;

namespace order_service.OrderService.Infrastructure.Models;

public partial class OrderDelivery
{
    public Guid OrderId { get; set; }

    public string ReceiverName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime? EstimatedTime { get; set; }

    public virtual Order Order { get; set; } = null!;
}
