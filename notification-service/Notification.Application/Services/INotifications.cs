using notification_service.Notifications.DTOS;

namespace notification_service.Notifications.Services
{
    public interface INotifications
    {

        public string TypeService { get; }
        Task<bool> SendNotification(RequestSendMessage request);

    }
}
