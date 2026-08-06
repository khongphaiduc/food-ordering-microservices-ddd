using food_service.ProductService.Application.DTOs;
using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Infrastructure.ImplementService;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Foodly.Tests.FoodService;

public class GetProductDailyInventoryTests
{
    private static readonly DateOnly InventoryDate = new(2026, 8, 6);

    [Fact]
    public async Task ExecuteAsync_IncludesConfiguredAndUnconfiguredProducts_WithAccurateSummary()
    {
        await using var db = CreateDbContext();
        var data = await SeedInventoryAsync(db);
        var service = new GetProductDailyInventory(db, new FixedInventoryDateProvider(InventoryDate));

        var result = await service.ExecuteAsync(new RequestGetProductDailyInventory
        {
            IncludeUnconfigured = true,
            SortBy = "productName"
        });

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(InventoryDate, result.InventoryDate);

        var unconfigured = Assert.Single(result.Items, item => item.ProductId == data.UnconfiguredProductId);
        Assert.False(unconfigured.IsConfigured);
        Assert.Equal(ProductDailyInventoryStatus.NotConfigured, unconfigured.InventoryStatus);
        Assert.Null(unconfigured.InventoryIsAvailable);
        Assert.Equal(100, unconfigured.InitialQuantity);

        var soldOut = Assert.Single(result.Items, item => item.ProductId == data.SoldOutProductId);
        Assert.True(soldOut.IsConfigured);
        Assert.False(soldOut.IsAvailable);
        Assert.Equal(ProductDailyInventoryStatus.SoldOut, soldOut.InventoryStatus);

        Assert.Equal(3, result.Summary.TotalProducts);
        Assert.Equal(2, result.Summary.ConfiguredProducts);
        Assert.Equal(1, result.Summary.UnconfiguredProducts);
        Assert.Equal(250, result.Summary.TotalInitialQuantity);
        Assert.Equal(130, result.Summary.TotalSoldQuantity);
        Assert.Equal(120, result.Summary.TotalRemainingQuantity);
        Assert.Equal(1, result.Summary.SoldOutProducts);
    }

    [Fact]
    public async Task ExecuteAsync_FiltersStatusAndUnconfiguredProducts_ThenSortsResults()
    {
        await using var db = CreateDbContext();
        var data = await SeedInventoryAsync(db);
        var service = new GetProductDailyInventory(db, new FixedInventoryDateProvider(InventoryDate));

        var result = await service.ExecuteAsync(new RequestGetProductDailyInventory
        {
            IncludeUnconfigured = false,
            Status = ProductDailyInventoryStatus.Available,
            SortBy = "remainingQuantity",
            SortDirection = "desc"
        });

        var item = Assert.Single(result.Items);
        Assert.Equal(data.AvailableProductId, item.ProductId);
        Assert.Equal(120, item.RemainingQuantity);
        Assert.True(item.IsAvailable);
        Assert.Equal(1, result.Summary.TotalProducts);
        Assert.Equal(1, result.Summary.ConfiguredProducts);
        Assert.Equal(0, result.Summary.UnconfiguredProducts);
    }

    private static FoodProductsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FoodProductsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FoodProductsDbContext(options);
    }

    private static async Task<InventoryTestData> SeedInventoryAsync(FoodProductsDbContext db)
    {
        var categoryId = Guid.NewGuid();
        var soldOutProductId = Guid.NewGuid();
        var availableProductId = Guid.NewGuid();
        var unconfiguredProductId = Guid.NewGuid();

        db.Categories.Add(new Category
        {
            Id = categoryId,
            Name = "Main dishes",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        db.Products.AddRange(
            CreateProduct(soldOutProductId, categoryId, "Sold out product"),
            CreateProduct(availableProductId, categoryId, "Available product"),
            CreateProduct(unconfiguredProductId, categoryId, "Unconfigured product"));

        db.ProductDailyInventories.AddRange(
            new ProductDailyInventory
            {
                Id = Guid.NewGuid(),
                ProductId = soldOutProductId,
                InventoryDate = InventoryDate,
                InitialQuantity = 100,
                RemainingQuantity = 0,
                SoldQuantity = 100,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductDailyInventory
            {
                Id = Guid.NewGuid(),
                ProductId = availableProductId,
                InventoryDate = InventoryDate,
                InitialQuantity = 150,
                RemainingQuantity = 120,
                SoldQuantity = 30,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        await db.SaveChangesAsync();

        return new InventoryTestData(
            soldOutProductId,
            availableProductId,
            unconfiguredProductId);
    }

    private static Product CreateProduct(Guid id, Guid categoryId, string name)
    {
        return new Product
        {
            Id = id,
            CategoryId = categoryId,
            Name = name,
            Price = 10,
            IsAvailable = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private sealed class FixedInventoryDateProvider : IInventoryDateProvider
    {
        public FixedInventoryDateProvider(DateOnly today)
        {
            Today = today;
        }

        public DateOnly Today { get; }
    }

    private sealed record InventoryTestData(
        Guid SoldOutProductId,
        Guid AvailableProductId,
        Guid UnconfiguredProductId);
}
