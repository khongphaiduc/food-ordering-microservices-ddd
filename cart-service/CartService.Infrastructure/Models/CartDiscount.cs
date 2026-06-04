using System;
using System.Collections.Generic;

namespace cart_service.CartService.Infrastructure.Models;

public partial class CartDiscount
{
    public Guid Id { get; set; }

    public Guid CartId { get; set; }

    public string Code { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public decimal AppliedAmount { get; set; }

    public virtual Cart Cart { get; set; } = null!;
}
