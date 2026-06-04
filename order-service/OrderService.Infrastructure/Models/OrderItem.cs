using System;
using System.Collections.Generic;

namespace order_service.OrderService.Infrastructure.Models;

public partial class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public Guid? VariantId { get; set; }

    public string? VariantName { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public string? OptionsSnapshot { get; set; }

    public virtual Order Order { get; set; } = null!;
}
