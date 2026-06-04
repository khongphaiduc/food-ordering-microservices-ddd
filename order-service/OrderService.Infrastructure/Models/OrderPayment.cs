using System;
using System.Collections.Generic;

namespace order_service.OrderService.Infrastructure.Models;

public partial class OrderPayment
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public string PaymentProvider { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string? TransactionId { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
