using Foodly.Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using order_service.OrderService.API.Proto;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Domain.Enums;
using payment_service.PaymentService.Infrastructure.NotificationRealTimeSignalR;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;

namespace payment_service.PaymentService.Infrastructure.Consumers
{
    public class PaymentConsumer : IConsumer<ReservedOrderSuccess>
    {
        private readonly PayOSClient _payos;
        private readonly OrderGrpc.OrderGrpcClient _orderClient;
        private readonly IPaymentRepository _payment;
        private readonly ILogger<PaymentConsumer> _logger;
        private readonly IHubContext<ViewQRCodeOrder> _hubcontext;

        public PaymentConsumer(IHubContext<ViewQRCodeOrder> hubContext, ILogger<PaymentConsumer> logger, PayOSClient payOSClient, OrderGrpc.OrderGrpcClient orderGrpcClient, IPaymentRepository paymentRepository)
        {
            _payos = payOSClient;
            _orderClient = orderGrpcClient;
            _payment = paymentRepository;
            _logger = logger;
            _hubcontext = hubContext;
        }

        public async Task Consume(ConsumeContext<ReservedOrderSuccess> context)
        {
            try
            {

                var order = await _orderClient.ViewOrderDetailAsync(new RequestOrder { IdOrder = context.Message.IdOrder.ToString() });
                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
                    Amount = (long)order.Amount,
                    Description = "2HONDAICODON",
                };
                var payloadPayment = await _payos.PaymentRequests.CreateAsync(paymentRequest);

                if (payloadPayment != null)
                {
                    var affected = await _payment.CreatePayment(new CreatePaymentPayload
                    {
                        OrderCode = payloadPayment.OrderCode.ToString(),
                        Amount = payloadPayment.Amount,
                        Currency = "VND",
                        PaymentMethods = PaymentMethod.PayOS,
                        Provider = "PAYOS",
                        UserId = Guid.NewGuid(),
                        QRCode = payloadPayment.QrCode,
                    });

                    await _hubcontext.Clients.User(context.Message.IdUser.ToString()).SendAsync("ViewQRCodeOrderMethod", $"{payloadPayment.QrCode}");
                    _logger.LogInformation($"Id User :{context.Message.IdUser}");
                    if (affected) _logger.LogInformation("Created Successfully a Payment");
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
