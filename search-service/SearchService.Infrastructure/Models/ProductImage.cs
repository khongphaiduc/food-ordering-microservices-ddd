using System;
using System.Collections.Generic;

namespace search_service.Models;

public partial class ProductImage
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsMain { get; set; }

    public virtual Product Product { get; set; } = null!;
}
