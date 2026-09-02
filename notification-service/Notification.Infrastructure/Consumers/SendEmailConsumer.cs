using Foodly.Contracts.Events;
using MassTransit;
using notification_service.Notifications.DTOS;
using notification_service.Notifications.Services;

namespace notification_service.Notification.Infrastructure.Consumers
{
    public class SendEmailConsumer : IConsumer<OrderCompletedEvent>
    {
        private readonly INotifications _notifi;
        private readonly ILogger<SendEmailConsumer> _logger;

        public SendEmailConsumer(IEnumerable<INotifications> notifications , ILogger<SendEmailConsumer> logger)
        {
            _notifi = notifications.First(s => s.TypeService == "Email");
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
            var message = context.Message;

            try
            {
                var subject = $"Xác nhận thanh toán đơn hàng #{message.OrderCode}";

                var body = $"""
            <div style="font-family: Arial, sans-serif; line-height: 1.6;">
                <h2>Xác nhận đơn hàng thành công</h2>

                <p>Xin chào <strong>{message.UserName}</strong>,</p>

                <p>
                    Cảm ơn bạn đã mua sắm tại <strong>Foodly</strong>.
                    Chúng tôi xin xác nhận rằng đơn hàng của bạn đã được
                    thanh toán thành công.
                </p>

                <p>
                    <strong>Mã đơn hàng:</strong> {message.OrderCode}<br/>
                    <strong>Tổng tiền:</strong> {message.TotalPrice:N0} VNĐ
                </p>

                <p>
                    Đơn hàng của bạn đã được ghi nhận và đang được xử lý.
                    Bạn có thể theo dõi trạng thái đơn hàng trong ứng dụng Foodly.
                </p>

                <p>
                    <strong>Lưu ý:</strong>
                    Để xem đầy đủ thông tin và chi tiết đơn hàng,
                    vui lòng truy cập ứng dụng Foodly.
                </p>

                <p>
                    Cảm ơn bạn đã tin tưởng và lựa chọn Foodly ❤️
                </p>

                <p>
                    Trân trọng,<br/>
                    <strong>Đội ngũ Foodly</strong>
                </p>
            </div>
            """;

                await _notifi.SendNotification(new RequestSendMessage
                {
                    To = message.Email,
                    Subject = subject,
                    Body = body,
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                _logger.LogError("Failed to send email notification for OrderCompletedEvent. OrderCode: {OrderCode}, Email: {Email}", message.OrderCode, message.Email);
                throw;
            }
        }


    }
}
