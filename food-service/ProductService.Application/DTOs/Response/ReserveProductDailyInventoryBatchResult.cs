namespace food_service.ProductService.Application.DTOs.Response;

public class ReserveProductDailyInventoryBatchResult
{
    public bool Success { get; set; }

    public Guid? FailedProductId { get; set; }

    public string Message { get; set; } = string.Empty;
}
