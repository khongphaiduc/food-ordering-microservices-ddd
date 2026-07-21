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

    public GetProductDailyInventory(FoodProductsDbContext foodProductsDbContext)
    {
        _db = foodProductsDbContext;
    }

    public async Task<PagedProductDailyInventoryDTO> ExecuteAsync(RequestGetProductDailyInventory request)
    {
        var inventoryDate = request.Date ?? DateOnly.FromDateTime(DateTime.Today);
        var pageIndex = Math.Max(request.PageIndex, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query =
            from product in _db.Products.AsNoTracking()
            join category in _db.Categories.AsNoTracking() on product.CategoryId equals category.Id
            join inventory in _db.ProductDailyInventories.AsNoTracking()
                    .Where(x => x.InventoryDate == inventoryDate)
                on product.Id equals inventory.ProductId into inventories
            from inventory in inventories.DefaultIfEmpty()
            where !product.IsDeleted
                && (!request.CategoryId.HasValue || product.CategoryId == request.CategoryId.Value)
            orderby category.Name, product.Name
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
                IsAvailable = product.IsAvailable && (inventory == null || inventory.IsAvailable)
            };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedProductDailyInventoryDTO
        {
            Items = items,
            TotalCount = totalCount
        };
    }
}
