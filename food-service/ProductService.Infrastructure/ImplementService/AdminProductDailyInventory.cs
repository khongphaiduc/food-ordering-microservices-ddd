using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Application.DTOs;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infrastructure.ImplementService;

public class AdminProductDailyInventory : IAdminProductDailyInventory
{
    private readonly FoodProductsDbContext _db;
    private readonly IInventoryDateProvider _dateProvider;

    public AdminProductDailyInventory(
        FoodProductsDbContext db,
        IInventoryDateProvider dateProvider)
    {
        _db = db;
        _dateProvider = dateProvider;
    }

    public async Task<AdminProductDailyInventoryResult> CreateAsync(
        CreateProductDailyInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryDate = request.InventoryDate ?? _dateProvider.Today;

        var insertedRows = await _db.Database.ExecuteSqlInterpolatedAsync($"""
    INSERT INTO product_daily_inventories
        (id, product_id, inventory_date, initial_quantity, remaining_quantity, sold_quantity, is_available, created_at, updated_at)
    SELECT gen_random_uuid(), p.id, {inventoryDate}, {request.InitialQuantity}, {request.InitialQuantity}, 0, {request.IsAvailable}, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
    FROM products AS p
    WHERE p.id = {request.ProductId}
      AND p.is_deleted = FALSE
    ON CONFLICT (product_id, inventory_date) DO NOTHING;
    """, cancellationToken);

        if (insertedRows == 1)
        {
            return Success(
                "Product daily inventory created successfully.",
                await LoadInventoryAsync(request.ProductId, inventoryDate, cancellationToken));
        }

        var productExists = await _db.Products
            .AsNoTracking()
            .AnyAsync(
                product => product.Id == request.ProductId && !product.IsDeleted,
                cancellationToken);

        if (!productExists)
        {
            return Failed(
                AdminInventoryOperationStatus.ProductNotFound,
                $"Product with ID {request.ProductId} was not found.");
        }

        return Failed(
            AdminInventoryOperationStatus.InventoryAlreadyExists,
            $"Inventory for product {request.ProductId} on {inventoryDate:yyyy-MM-dd} already exists.");
    }

    public async Task<AdminProductDailyInventoryResult> RestockAsync(
        RestockProductDailyInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var inventoryDate = request.InventoryDate ?? _dateProvider.Today;

        var updatedRows = await _db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE product_daily_inventories
            SET initial_quantity = initial_quantity + {request.Quantity},
                remaining_quantity = remaining_quantity + {request.Quantity},
                updated_at = CURRENT_TIMESTAMP
            WHERE product_id = {request.ProductId}
              AND inventory_date = {inventoryDate}
              AND initial_quantity <= {int.MaxValue - request.Quantity}
              AND remaining_quantity <= {int.MaxValue - request.Quantity};
            """, cancellationToken);

        if (updatedRows == 1)
        {
            return Success(
                "Product daily inventory restocked successfully.",
                await LoadInventoryAsync(request.ProductId, inventoryDate, cancellationToken));
        }

        var inventoryExists = await _db.ProductDailyInventories
            .AsNoTracking()
            .AnyAsync(
                inventory => inventory.ProductId == request.ProductId
                    && inventory.InventoryDate == inventoryDate,
                cancellationToken);

        if (!inventoryExists)
        {
            return Failed(
                AdminInventoryOperationStatus.InventoryNotFound,
                $"Inventory for product {request.ProductId} on {inventoryDate:yyyy-MM-dd} was not found.");
        }

        return Failed(
            AdminInventoryOperationStatus.QuantityLimitExceeded,
            "The restock quantity would exceed the supported inventory limit.");
    }

    private async Task<ProductDailyInventoryDTO?> LoadInventoryAsync(
        Guid productId,
        DateOnly inventoryDate,
        CancellationToken cancellationToken)
    {
        return await (
            from inventory in _db.ProductDailyInventories.AsNoTracking()
            join product in _db.Products.AsNoTracking() on inventory.ProductId equals product.Id
            join category in _db.Categories.AsNoTracking() on product.CategoryId equals category.Id
            where inventory.ProductId == productId && inventory.InventoryDate == inventoryDate
            select new ProductDailyInventoryDTO
            {
                ProductId = product.Id,
                ProductName = product.Name,
                CategoryId = category.Id,
                CategoryName = category.Name,
                InventoryDate = inventory.InventoryDate,
                InitialQuantity = inventory.InitialQuantity,
                RemainingQuantity = inventory.RemainingQuantity,
                SoldQuantity = inventory.SoldQuantity,
                IsConfigured = true,
                ProductIsAvailable = product.IsAvailable,
                InventoryIsAvailable = inventory.IsAvailable,
                InventoryStatus = !product.IsAvailable
                    ? ProductDailyInventoryStatus.ProductUnavailable
                    : !inventory.IsAvailable
                        ? ProductDailyInventoryStatus.Disabled
                        : inventory.RemainingQuantity <= 0
                            ? ProductDailyInventoryStatus.SoldOut
                            : ProductDailyInventoryStatus.Available,
                IsAvailable = product.IsAvailable
                    && inventory.IsAvailable
                    && inventory.RemainingQuantity > 0
            }).FirstOrDefaultAsync(cancellationToken);
    }

    private static AdminProductDailyInventoryResult Success(
        string message,
        ProductDailyInventoryDTO? inventory)
    {
        return new AdminProductDailyInventoryResult
        {
            Status = AdminInventoryOperationStatus.Success,
            Message = message,
            Inventory = inventory
        };
    }

    private static AdminProductDailyInventoryResult Failed(
        AdminInventoryOperationStatus status,
        string message)
    {
        return new AdminProductDailyInventoryResult
        {
            Status = status,
            Message = message
        };
    }
}
