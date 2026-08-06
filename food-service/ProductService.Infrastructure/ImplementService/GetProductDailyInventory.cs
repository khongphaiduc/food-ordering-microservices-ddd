using food_service.ProductService.Application.DTOs;
using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infrastructure.ImplementService;

public class GetProductDailyInventory : IGetProductDailyInventory
{
    private const int DefaultDailyQuantity = 100;
    private readonly FoodProductsDbContext _db;
    private readonly IInventoryDateProvider _dateProvider;

    public GetProductDailyInventory(
        FoodProductsDbContext foodProductsDbContext,
        IInventoryDateProvider dateProvider)
    {
        _db = foodProductsDbContext;
        _dateProvider = dateProvider;
    }

    public async Task<PagedProductDailyInventoryDTO> ExecuteAsync(
        RequestGetProductDailyInventory request,
        CancellationToken cancellationToken = default)
    {
        var inventoryDate = request.Date ?? _dateProvider.Today;
        var pageIndex = Math.Max(request.PageIndex, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var products = _db.Products
            .AsNoTracking()
            .Where(product => !product.IsDeleted);

        if (request.CategoryId.HasValue)
        {
            products = products.Where(product => product.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keywordPattern = $"%{request.Keyword.Trim()}%";
            products = products.Where(product => EF.Functions.ILike(product.Name, keywordPattern));
        }

        IQueryable<ProductDailyInventoryDTO> query =
            from product in products
            join category in _db.Categories.AsNoTracking() on product.CategoryId equals category.Id
            join inventory in _db.ProductDailyInventories.AsNoTracking()
                    .Where(inventory => inventory.InventoryDate == inventoryDate)
                on product.Id equals inventory.ProductId into inventories
            from inventory in inventories.DefaultIfEmpty()
            select new ProductDailyInventoryDTO
            {
                ProductId = product.Id,
                ProductName = product.Name,
                CategoryId = category.Id,
                CategoryName = category.Name,
                InventoryDate = inventoryDate,
                InitialQuantity = inventory == null ? DefaultDailyQuantity : inventory.InitialQuantity,
                RemainingQuantity = inventory == null ? DefaultDailyQuantity : inventory.RemainingQuantity,
                SoldQuantity = inventory == null ? 0 : inventory.SoldQuantity,
                IsConfigured = inventory != null,
                ProductIsAvailable = product.IsAvailable,
                InventoryIsAvailable = inventory == null ? null : inventory.IsAvailable,
                InventoryStatus = inventory == null
                    ? ProductDailyInventoryStatus.NotConfigured
                    : !product.IsAvailable
                        ? ProductDailyInventoryStatus.ProductUnavailable
                        : !inventory.IsAvailable
                            ? ProductDailyInventoryStatus.Disabled
                            : inventory.RemainingQuantity <= 0
                                ? ProductDailyInventoryStatus.SoldOut
                                : ProductDailyInventoryStatus.Available,
                IsAvailable = inventory != null
                    && product.IsAvailable
                    && inventory.IsAvailable
                    && inventory.RemainingQuantity > 0
            };

        if (!request.IncludeUnconfigured)
        {
            query = query.Where(inventory => inventory.IsConfigured);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(inventory => inventory.InventoryStatus == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var summary = await query
            .GroupBy(_ => 1)
            .Select(group => new ProductDailyInventorySummaryDTO
            {
                TotalProducts = group.Count(),
                ConfiguredProducts = group.Count(inventory => inventory.IsConfigured),
                UnconfiguredProducts = group.Count(inventory => !inventory.IsConfigured),
                TotalInitialQuantity = group.Sum(inventory =>
                    inventory.IsConfigured ? (long)inventory.InitialQuantity : 0L),
                TotalSoldQuantity = group.Sum(inventory =>
                    inventory.IsConfigured ? (long)inventory.SoldQuantity : 0L),
                TotalRemainingQuantity = group.Sum(inventory =>
                    inventory.IsConfigured ? (long)inventory.RemainingQuantity : 0L),
                SoldOutProducts = group.Count(inventory =>
                    inventory.InventoryStatus == ProductDailyInventoryStatus.SoldOut)
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? new ProductDailyInventorySummaryDTO();

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedProductDailyInventoryDTO
        {
            Items = items,
            TotalCount = totalCount,
            InventoryDate = inventoryDate,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Summary = summary
        };
    }

    private static IQueryable<ProductDailyInventoryDTO> ApplySorting(
        IQueryable<ProductDailyInventoryDTO> query,
        string sortBy,
        string sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "initialquantity" => descending
                ? query.OrderByDescending(item => item.InitialQuantity).ThenBy(item => item.ProductName)
                : query.OrderBy(item => item.InitialQuantity).ThenBy(item => item.ProductName),
            "remainingquantity" => descending
                ? query.OrderByDescending(item => item.RemainingQuantity).ThenBy(item => item.ProductName)
                : query.OrderBy(item => item.RemainingQuantity).ThenBy(item => item.ProductName),
            "soldquantity" => descending
                ? query.OrderByDescending(item => item.SoldQuantity).ThenBy(item => item.ProductName)
                : query.OrderBy(item => item.SoldQuantity).ThenBy(item => item.ProductName),
            _ => descending
                ? query.OrderByDescending(item => item.ProductName)
                : query.OrderBy(item => item.ProductName)
        };
    }
}
