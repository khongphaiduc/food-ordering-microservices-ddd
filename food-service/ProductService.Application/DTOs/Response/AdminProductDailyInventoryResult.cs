namespace food_service.ProductService.Application.DTOs.Response;

public enum AdminInventoryOperationStatus
{
    Success,
    ProductNotFound,
    InventoryAlreadyExists,
    InventoryNotFound,
    QuantityLimitExceeded
}

public class AdminProductDailyInventoryResult
{
    public AdminInventoryOperationStatus Status { get; set; }

    public string Message { get; set; } = string.Empty;

    public ProductDailyInventoryDTO? Inventory { get; set; }
}
