using food_service.ProductService.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace food_service.ProductService.Application.DTOs.Request;

public class RequestGetProductDailyInventory
{
    public DateOnly? Date { get; set; }

    public Guid? CategoryId { get; set; }

    public string? Keyword { get; set; }

    public ProductDailyInventoryStatus? Status { get; set; }

    public bool IncludeUnconfigured { get; set; } = true;

    [RegularExpression(
        "^(productName|initialQuantity|remainingQuantity|soldQuantity)$",
        ErrorMessage = "SortBy must be productName, initialQuantity, remainingQuantity, or soldQuantity.")]
    public string SortBy { get; set; } = "productName";

    [RegularExpression(
        "^(asc|desc)$",
        ErrorMessage = "SortDirection must be asc or desc.")]
    public string SortDirection { get; set; } = "asc";

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
