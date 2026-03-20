using tracking_service.Tracking.Application.DTO;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Domain.Aggregate;
using tracking_service.Tracking.Domain.Repository;

namespace tracking_service.Tracking.Infastructrure.ImplementServices
{
    public class UserBehaviorTracking : IUserBehaviorTracking
    {
        private readonly IUserSessionRepository _sessionRepository;

        public UserBehaviorTracking(IUserSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task Execute(TrackingDTO request)
        {
           
            bool sessionExists = await _sessionRepository.SessionExists(request.IdSession);

            if (!sessionExists)  // exists no session, create a new one and add the event
            {
               
                var newSession = UserSessionAggregate.CreateNewSessoionUser(request.IdUser);

                
                newSession.AddEvent(request.EventType.ToString(), request.IdProduct);

                
                await _sessionRepository.AddNewUserSession(newSession);
            }
            else
            {
               
                var existingEvent = new Domain.Entities.TrackingEvent(
                    request.IdUser,
                    request.IdSession,
                    request.EventType.ToString(),
                    request.IdProduct
                );

                await _sessionRepository.AddEvent(existingEvent);
            }
        }
    }
}