using System;
using System.Collections.Generic;

namespace search_service.Models;

public partial class ProductVariant
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal ExtraPrice { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
