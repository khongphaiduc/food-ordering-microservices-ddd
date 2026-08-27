using Foodly.Contracts.Events;
using MassTransit;
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
        private readonly IPublishEndpoint _publishEvent;

        public UpdateOrderStatus(IPublishEndpoint publishEndpoint, IPaymentRepository paymentRepository, OrderServiceUpdateStatusGrpc.OrderServiceUpdateStatusGrpcClient orderServiceUpdateStatusGrpcClient, ILogger<UpdateOrderStatus> logger, IHubContext<NotificationPaidSuccessfully> hubContext)
        {
            _orderService = orderServiceUpdateStatusGrpcClient;
            _logger = logger;
            _notificationPayOs = hubContext;
            _payment = paymentRepository;
            _publishEvent = publishEndpoint;
        }

        public async Task<bool> Excute(WebhookData webhookData)
        {

            // To push message to broker (tranfer a message to order ) when a user completed the payment
            await _publishEvent.Publish(new PaySuccessfullyEvent
            {
                OrderCode = webhookData.OrderCode.ToString(),
                Happen = DateTime.UtcNow,
            });

            _logger.LogInformation($"Order code is : {webhookData.OrderCode}");
            return await _payment.UpdateStatusPayment(webhookData.OrderCode.ToString(), Domain.Enums.PaymentStatus.Succeeded);
        }

    }
}
