using food_service.ProductService.Application.DTOs;
using System.Text.Json.Serialization;

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
    public bool IsConfigured { get; set; }
    public bool ProductIsAvailable { get; set; }
    public bool? InventoryIsAvailable { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductDailyInventoryStatus InventoryStatus { get; set; }

    public bool IsAvailable { get; set; }
}

public class ProductDailyInventorySummaryDTO
{
    public int TotalProducts { get; set; }

    public int ConfiguredProducts { get; set; }

    public int UnconfiguredProducts { get; set; }

    public long TotalInitialQuantity { get; set; }

    public long TotalSoldQuantity { get; set; }

    public long TotalRemainingQuantity { get; set; }

    public int SoldOutProducts { get; set; }
}

public class PagedProductDailyInventoryDTO
{
    public List<ProductDailyInventoryDTO> Items { get; set; } = [];

    public int TotalCount { get; set; }

    public DateOnly InventoryDate { get; set; }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public ProductDailyInventorySummaryDTO Summary { get; set; } = new();
}
