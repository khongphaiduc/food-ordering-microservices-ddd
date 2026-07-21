using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infrastructure.ImplementService;

public class ReserveProductDailyInventory : IReserveProductDailyInventory
{
    private const int DefaultDailyQuantity = 100;
    private readonly FoodProductsDbContext _db;

    public ReserveProductDailyInventory(FoodProductsDbContext foodProductsDbContext)
    {
        _db = foodProductsDbContext;
    }

    public async Task<bool> ExecuteAsync(ReserveProductDailyInventoryRequest request)
    {
        var result = await ExecuteBatchAsync([request]);
        return result.Success;
    }


    // To reduce inventory
    public async Task<ReserveProductDailyInventoryBatchResult> ExecuteBatchAsync(IReadOnlyCollection<ReserveProductDailyInventoryRequest> requests)
    {
        if (requests.Count == 0)
        {
            return Failed(Guid.Empty, "At least one inventory item is required.");
        }

        var inventoryRequests = new List<(Guid ProductId, int Quantity, DateOnly InventoryDate)>();
        foreach (var request in requests)
        {
            if (request.ProductId == Guid.Empty)
            {
                return Failed(request.ProductId, "ProductId is required.");
            }

            if (request.Quantity <= 0 || request.Quantity > DefaultDailyQuantity)
            {
                return Failed(request.ProductId, $"Quantity must be between 1 and {DefaultDailyQuantity}.");
            }

            inventoryRequests.Add((
                request.ProductId,
                request.Quantity,
                request.InventoryDate ?? DateOnly.FromDateTime(DateTime.Today)));
        }

        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            foreach (var request in inventoryRequests)
            {
                if (await ReserveAsync(request.ProductId, request.Quantity, request.InventoryDate) != 1)
                {
                    await transaction.RollbackAsync();
                    return Failed(request.ProductId, "Inventory is unavailable or insufficient.");
                }
            }

            await transaction.CommitAsync();
            return new ReserveProductDailyInventoryBatchResult
            {
                Success = true,
                Message = "All inventory items were reserved successfully."
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


   
    private async Task<int> ReserveAsync(Guid productId, int quantity, DateOnly inventoryDate)
    {
        return await _db.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO product_daily_inventories
                (id, product_id, inventory_date, initial_quantity, remaining_quantity, sold_quantity, is_available, created_at, updated_at)
            SELECT uuidv7(), p.id, {inventoryDate}, {DefaultDailyQuantity}, {DefaultDailyQuantity - quantity}, {quantity}, TRUE, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
            FROM products AS p
            WHERE p.id = {productId}
              AND p.is_deleted = FALSE
              AND p.is_available = TRUE
            ON CONFLICT (product_id, inventory_date) DO UPDATE
            SET remaining_quantity = product_daily_inventories.remaining_quantity - {quantity},
                sold_quantity = product_daily_inventories.sold_quantity + {quantity},
                updated_at = CURRENT_TIMESTAMP
            WHERE product_daily_inventories.is_available = TRUE
              AND product_daily_inventories.remaining_quantity >= {quantity};
            """);
    }

    private static ReserveProductDailyInventoryBatchResult Failed(Guid productId, string message)
    {
        return new ReserveProductDailyInventoryBatchResult
        {
            Success = false,
            FailedProductId = productId == Guid.Empty ? null : productId,
            Message = message
        };
    }
}
