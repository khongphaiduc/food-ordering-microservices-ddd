using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infrastructure.Models;
using order_service.OrderService.Infrastructure.ServicesImplements;

namespace Foodly.Tests.OrderService;

public class UpdateStatusOrdersTests
{
    [Fact]
    public async Task Execute_OrderDoesNotExist_ReturnsFalse()
    {
        await using var db = CreateContext();
        var service = new UpdateStatusOrders(db);

        var result = await service.Execute(new RequestUpdateStatusOrder
        {
            IdOrder = Guid.NewGuid(),
            Status = OrderStatus.CANCELLED
        });

        Assert.False(result);
    }

    [Fact]
    public async Task Execute_CancelledStatus_UpdatesPaymentAndOrderStatus()
    {
        await using var db = CreateContext();
        var order = CreateOrder(PaymentMethod.PayOS.ToString());
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        var service = new UpdateStatusOrders(db);

        var result = await service.Execute(new RequestUpdateStatusOrder
        {
            IdOrder = order.Id,
            Status = OrderStatus.CANCELLED
        });

        Assert.True(result);
        Assert.Equal(OrderStatusPayment.CANCELLED.ToString(), order.Status);
        Assert.Equal(OrderStatus.CANCELLED.ToString(), order.OrderStatus);
    }

    [Fact]
    public async Task Execute_CompletedCashOrder_MarksPaymentAsPaid()
    {
        await using var db = CreateContext();
        var order = CreateOrder(PaymentMethod.Cash.ToString());
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        var service = new UpdateStatusOrders(db);

        var result = await service.Execute(new RequestUpdateStatusOrder
        {
            IdOrder = order.Id,
            Status = OrderStatus.COMPLETED
        });

        Assert.True(result);
        Assert.Equal(OrderStatusPayment.PAID.ToString(), order.Status);
        Assert.Equal(OrderStatus.COMPLETED.ToString(), order.OrderStatus);
    }

    private static Order CreateOrder(string paymentMethod)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CartId = Guid.NewGuid(),
            OrderCode = Guid.NewGuid().ToString("N"),
            Status = OrderStatusPayment.PENDING.ToString(),
            PaymentMethod = paymentMethod,
            TotalAmount = 100,
            ShippingFee = 10,
            DiscountAmount = 0,
            FinalAmount = 110,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderStatus = OrderStatus.PENDING.ToString()
        };
    }

    private static FoodOrderContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FoodOrderContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FoodOrderContext(options);
    }
}
