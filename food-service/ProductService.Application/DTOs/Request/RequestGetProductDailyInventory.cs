namespace food_service.ProductService.Application.DTOs.Request;

public class RequestGetProductDailyInventory
{
    public DateOnly? Date { get; set; }

    public Guid? CategoryId { get; set; }

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
