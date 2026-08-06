using System.ComponentModel.DataAnnotations;

namespace food_service.ProductService.Application.DTOs.Request;

public class CreateProductDailyInventoryRequest
{
    public Guid ProductId { get; set; }

    public DateOnly? InventoryDate { get; set; }

    [Range(0, int.MaxValue)]
    public int InitialQuantity { get; set; } = 100;

    public bool IsAvailable { get; set; } = true;
}
