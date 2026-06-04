using System;
using System.Collections.Generic;

namespace cart_service.CartService.Infrastructure.Models;

public partial class CartItem
{
    public Guid Id { get; set; }

    public Guid CartId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? VariantName { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Cart Cart { get; set; } = null!;
}
