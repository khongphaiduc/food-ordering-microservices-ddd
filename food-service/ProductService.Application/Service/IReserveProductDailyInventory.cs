using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Service;

public interface IReserveProductDailyInventory
{

    /// <summary>
    /// Reserves the requested quantity for one product on the specified inventory date.
    /// </summary>
    /// <param name="request">The product, quantity, and optional inventory date to reserve.</param>
    /// <returns>
    /// <see langword="true"/> when the inventory was reserved; otherwise, <see langword="false"/>
    /// when the product is invalid, unavailable for sale, or has insufficient inventory.
    /// </returns>
    Task<bool> ExecuteAsync(ReserveProductDailyInventoryRequest request);

    /// <summary>
    /// Reserves inventory for all requested products as a single atomic batch.
    /// </summary>
    /// <param name="requests">The inventory reservations to process.</param>
    /// <returns>
    /// A result that identifies whether the full batch succeeded; if one reservation fails,
    /// no reservation in the batch is persisted.
    /// </returns>
    Task<ReserveProductDailyInventoryBatchResult> ExecuteBatchAsync(IReadOnlyCollection<ReserveProductDailyInventoryRequest> requests);
}
