using System;
using System.Collections.Generic;

namespace payment_service.PaymentService.Infrastructure.Models;

public partial class PaymentTransaction
{
    public Guid Id { get; set; }

    public Guid PaymentId { get; set; }

    public string? ProviderTransactionId { get; set; }

    public string? Status { get; set; }

    public string? RawResponse { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
