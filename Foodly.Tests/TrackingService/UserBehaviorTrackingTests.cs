using Moq;
using tracking_service.Tracking.Application.DTO;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Domain.Enums;
using tracking_service.Tracking.Infastructrure.ImplementServices;

namespace Foodly.Tests.TrackingService;

public class UserBehaviorTrackingTests
{
    [Fact]
    public async Task Execute_ForwardsTrackingRequestToProducer()
    {
        var request = new TrackingDTO
        {
            IdSession = Guid.NewGuid(),
            IdUser = Guid.NewGuid(),
            PayLoad = new List<TrackingPayload>
            {
                new() { EventType = EventType.ViewProduct, IdProduct = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
            }
        };

        var producer = new Mock<IProducerTracking>();
        var service = new UserBehaviorTracking(producer.Object);

        await service.Execute(request);

        producer.Verify(x => x.SendMessage(request), Times.Once);
    }
}
