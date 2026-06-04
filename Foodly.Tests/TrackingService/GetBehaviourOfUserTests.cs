using Microsoft.EntityFrameworkCore;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Infrastructure.ImplementServices;
using tracking_service.Tracking.Infrastructure.Models;

namespace Foodly.Tests.TrackingService;

public class GetBehaviourOfUserTests
{
    [Fact]
    public async Task Execute_NoSession_ReturnsEmptyBehaviour()
    {
        await using var db = CreateContext();
        var service = new GetBehaviourOfUser(db);

        var result = await service.Execute(Guid.NewGuid());

        Assert.Empty(result.BehaviourUsers ?? new List<DataOfUsers>());
    }

    [Fact]
    public async Task Execute_UsesLatestSessionAndGroupsEventsByTypeAndProduct()
    {
        await using var db = CreateContext();
        var userId = Guid.NewGuid();
        var olderSessionId = Guid.NewGuid();
        var latestSessionId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        db.UserSessions.AddRange(
            new UserSession { Id = olderSessionId, UserId = userId, StartedAt = DateTime.UtcNow.AddDays(-1) },
            new UserSession { Id = latestSessionId, UserId = userId, StartedAt = DateTime.UtcNow });
        db.TrackingEvents.AddRange(
            new TrackingEvent { UserId = userId, SessionId = olderSessionId, EventType = "ViewProduct", ProductId = Guid.NewGuid() },
            new TrackingEvent { UserId = userId, SessionId = latestSessionId, EventType = "ViewProduct", ProductId = productId },
            new TrackingEvent { UserId = userId, SessionId = latestSessionId, EventType = "ViewProduct", ProductId = productId },
            new TrackingEvent { UserId = userId, SessionId = latestSessionId, EventType = "AddToCart", ProductId = productId });
        await db.SaveChangesAsync();

        var service = new GetBehaviourOfUser(db);

        var result = await service.Execute(userId);

        Assert.Equal(userId, result.IdUser);
        Assert.Equal(2, result.BehaviourUsers.Count);
        Assert.Contains(result.BehaviourUsers, x => x.eventType == "ViewProduct" && x.IdProduct == productId && x.CountTimes == 2);
        Assert.Contains(result.BehaviourUsers, x => x.eventType == "AddToCart" && x.IdProduct == productId && x.CountTimes == 1);
    }

    private static FoodProductsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FoodProductsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FoodProductsDbContext(options);
    }
}
