namespace order_service.OrderService.Application.DTOs.DTOsInternal;

public class InventoryReservationResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<InventoryReservationItemResult> Items { get; set; } = [];
}

public class InventoryReservationItemResult
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int RequestedQuantity { get; set; }

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}
