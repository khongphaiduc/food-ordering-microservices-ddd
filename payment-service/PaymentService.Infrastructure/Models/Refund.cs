using System;
using System.Collections.Generic;

namespace payment_service.PaymentService.Infrastructure.Models;

public partial class Refund
{
    public Guid Id { get; set; }

    public Guid PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string? ProviderRefundId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
