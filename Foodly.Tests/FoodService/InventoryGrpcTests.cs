extern alias FoodServiceProject;

using food_service.ProductService.API.gRPC;
using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Service;
using Moq;
using FoodReserveDailyInventoryItem = FoodServiceProject::productService.API.Protos.ReserveDailyInventoryItem;
using FoodReserveDailyInventoryRequest = FoodServiceProject::productService.API.Protos.ReserveDailyInventoryRequest;

namespace Foodly.Tests.FoodService;

public class InventoryGrpcTests
{
    [Fact]
    public async Task ReserveDailyInventory_InvalidItem_ReturnsMessageForEveryRequestedProduct()
    {
        var reserve = new Mock<IReserveProductDailyInventory>();
        var service = new Inventory(reserve.Object);
        var validProductId = Guid.NewGuid();
        var request = new FoodReserveDailyInventoryRequest();
        request.Items.Add(new FoodReserveDailyInventoryItem
        {
            ProductId = "not-a-guid",
            Quantity = 1
        });
        request.Items.Add(new FoodReserveDailyInventoryItem
        {
            ProductId = validProductId.ToString(),
            Quantity = 2
        });

        var response = await service.ReserveDailyInventory(request, null!);

        Assert.False(response.Success);
        Assert.Contains("invalid", response.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(2, response.Items.Count);
        Assert.Contains("Invalid product_id", response.Items[0].Message);
        Assert.Contains("request contains invalid items", response.Items[1].Message);
        reserve.Verify(
            item => item.ExecuteBatchAsync(It.IsAny<IReadOnlyCollection<ReserveProductDailyInventoryRequest>>()),
            Times.Never);
    }

    [Fact]
    public async Task ReserveDailyInventory_InsufficientProduct_ReturnsSpecificAndRollbackMessages()
    {
        var failedProductId = Guid.NewGuid();
        var rolledBackProductId = Guid.NewGuid();
        var reserve = new Mock<IReserveProductDailyInventory>();
        reserve
            .Setup(item => item.ExecuteBatchAsync(
                It.IsAny<IReadOnlyCollection<ReserveProductDailyInventoryRequest>>()))
            .ReturnsAsync(new ReserveProductDailyInventoryBatchResult
            {
                Success = false,
                FailedProductId = failedProductId,
                Message = "Inventory is unavailable or insufficient."
            });

        var service = new Inventory(reserve.Object);
        var request = new FoodReserveDailyInventoryRequest();
        request.Items.Add(new FoodReserveDailyInventoryItem
        {
            ProductId = failedProductId.ToString(),
            Quantity = 101
        });
        request.Items.Add(new FoodReserveDailyInventoryItem
        {
            ProductId = rolledBackProductId.ToString(),
            Quantity = 1
        });

        var response = await service.ReserveDailyInventory(request, null!);

        Assert.False(response.Success);
        Assert.Equal("Inventory is unavailable or insufficient.", response.Message);
        Assert.Equal(2, response.Items.Count);
        Assert.Equal("Inventory is unavailable or insufficient.", response.Items[0].Message);
        Assert.Contains("batch was rolled back", response.Items[1].Message);
        Assert.All(response.Items, item => Assert.False(item.Success));
    }
}
