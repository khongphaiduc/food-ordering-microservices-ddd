namespace food_service.ProductService.Application.DTOs.Request;

public class ReserveProductDailyInventoryRequest
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    // Không truyền sẽ dùng ngày hiện tại của server.
    public DateOnly? InventoryDate { get; set; }
}
