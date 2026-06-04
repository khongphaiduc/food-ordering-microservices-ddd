using System;
using System.Collections.Generic;

namespace cart_service.CartService.Infrastructure.Models;

public partial class Cart
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Status { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CartDiscount> CartDiscounts { get; set; } = new List<CartDiscount>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
