namespace food_service.ProductService.Infrastructure.Models;

public partial class ProductDailyInventory
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public DateOnly InventoryDate { get; set; }

    public int InitialQuantity { get; set; }

    public int RemainingQuantity { get; set; }

    public int SoldQuantity { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
