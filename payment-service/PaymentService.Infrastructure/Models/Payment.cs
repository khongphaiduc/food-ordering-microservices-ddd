using System;
using System.Collections.Generic;

namespace payment_service.PaymentService.Infrastructure.Models;

public partial class Payment
{
    public Guid Id { get; set; }

    public string OrderCode { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }



    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
