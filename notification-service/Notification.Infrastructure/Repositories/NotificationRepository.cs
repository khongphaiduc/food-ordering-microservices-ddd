using notification_service.Models;
using notification_service.Notification.Domain.Aggregate;
using notification_service.Notification.Domain.Interface;
using notification_service.Notifications.DTOS;

namespace notification_service.Notification.Infrastructure.Repositories
{
    public enum NotificationType
    {
        Email,
        SMS,
        Push
    }

    public enum NotificationStatus
    {
        Pending,
        Sent,
        Failed,
        Retrying
    }
    public class NotificationRepository : INotificationRepository
    {
        private readonly FoodNotificationDbContext _db;

        public NotificationRepository(FoodNotificationDbContext foodNotificationDbContext)
        {
            _db = foodNotificationDbContext;
        }

        public async Task<bool> AddNewRecordNotification(RequestSendMessage message)
        {
            var notificationAggregate = NotificationAggregate.CreateNewNotification(message.TypeService, message.To, message.Body, "Pending", "No Response");
            var notification = new Models.Notification
            {
                Id = notificationAggregate.Id,
                Userid = Guid.NewGuid(),
                Content = notificationAggregate.Message,
                Recipient = notificationAggregate.Recipient,
                Providerresponse = notificationAggregate.ProvideResponse,
                Createdat = notificationAggregate.CreateAt,
                Updatedat = notificationAggregate.UpdateAt
            };

            _db.Notifications.Add(notification);
            _db.Entry(notification).Property("Type").CurrentValue = "Email";
            _db.Entry(notification).Property("Status").CurrentValue = "Sent";
            return await _db.SaveChangesAsync() > 0;
        }

    }
}
