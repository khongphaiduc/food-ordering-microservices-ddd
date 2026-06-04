using tracking_service.Tracking.Application.DTO;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Domain.Aggregate;
using tracking_service.Tracking.Domain.Repository;

namespace tracking_service.Tracking.Infrastructure.ImplementServices
{
    public class UserBehaviorTracking : IUserBehaviorTracking
    {
        private readonly IProducerTracking _producerTracking;

        public UserBehaviorTracking(IProducerTracking producerTracking)
        {

            _producerTracking = producerTracking;
        }

        public async Task Execute(TrackingDTO request)
        {
            await _producerTracking.SendMessage(request);
        }
    }
}