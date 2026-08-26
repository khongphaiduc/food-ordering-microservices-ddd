using Microsoft.AspNetCore.SignalR;
using OrderService.API.Proto;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Infrastructure.NotificationRealTimeSignalR;
using PayOS.Models.Webhooks;

namespace payment_service.PaymentService.Infrastructure.ServicesImplements
{
    public class UpdateOrderStatus : IUpdateOrderStatus
    {
        private readonly OrderServiceUpdateStatusGrpc.OrderServiceUpdateStatusGrpcClient _orderService;
        private readonly ILogger<UpdateOrderStatus> _logger;
        private readonly IHubContext<NotificationPaidSuccessfully> _notificationPayOs;
        private readonly IPaymentRepository _payment;

        public UpdateOrderStatus(IPaymentRepository paymentRepository, OrderServiceUpdateStatusGrpc.OrderServiceUpdateStatusGrpcClient orderServiceUpdateStatusGrpcClient, ILogger<UpdateOrderStatus> logger, IHubContext<NotificationPaidSuccessfully> hubContext)
        {
            _orderService = orderServiceUpdateStatusGrpcClient;
            _logger = logger;
            _notificationPayOs = hubContext;
            _payment = paymentRepository;
        }

        public async Task<bool> Excute(WebhookData webhookData)
        {
            _logger.LogInformation($"Order code is : {webhookData.OrderCode}");
            return await _payment.UpdateStatusPayment(webhookData.OrderCode.ToString(), Domain.Enums.PaymentStatus.Succeeded);
        }

    }
}
