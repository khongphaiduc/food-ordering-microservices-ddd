using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Service;

public interface IGetProductDailyInventory
{
    Task<PagedProductDailyInventoryDTO> ExecuteAsync(
        RequestGetProductDailyInventory request,
        CancellationToken cancellationToken = default);
}
