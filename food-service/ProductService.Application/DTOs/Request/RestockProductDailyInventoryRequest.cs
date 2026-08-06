using System.ComponentModel.DataAnnotations;

namespace food_service.ProductService.Application.DTOs.Request;

public class RestockProductDailyInventoryRequest
{
    public Guid ProductId { get; set; }

    public DateOnly? InventoryDate { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
