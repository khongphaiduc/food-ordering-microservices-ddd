namespace food_service.ProductService.Application.DTOs.Response;

public class ProductDailyInventoryDTO
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public DateOnly InventoryDate { get; set; }
    public int InitialQuantity { get; set; }
    public int RemainingQuantity { get; set; }
    public int SoldQuantity { get; set; }
    public bool IsAvailable { get; set; }
}

public class PagedProductDailyInventoryDTO
{
    public List<ProductDailyInventoryDTO> Items { get; set; } = [];

    public int TotalCount { get; set; }
}
