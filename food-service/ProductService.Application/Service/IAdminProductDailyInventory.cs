using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Service;

public interface IAdminProductDailyInventory
{
    Task<AdminProductDailyInventoryResult> CreateAsync(
        CreateProductDailyInventoryRequest request,
        CancellationToken cancellationToken = default);

    Task<AdminProductDailyInventoryResult> RestockAsync(
        RestockProductDailyInventoryRequest request,
        CancellationToken cancellationToken = default);
}
