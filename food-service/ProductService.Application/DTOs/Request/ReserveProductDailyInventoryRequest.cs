namespace food_service.ProductService.Application.DTOs.Request;

public class ReserveProductDailyInventoryRequest
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public DateOnly? InventoryDate { get; set; }
}
