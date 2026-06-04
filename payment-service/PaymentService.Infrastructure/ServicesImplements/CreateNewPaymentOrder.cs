
using payment_service.PaymentService.API.gRPC;
using payment_service.PaymentService.Application.DTOs;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Infrastructure.Models;
using PayOS;
using PayOS.Models.V2.PaymentRequests;

namespace payment_service.PaymentService.Infrastructure.ServicesImplements
{
    public class CreateNewPaymentOrder : ICreateNewPaymentOrder
    {
        private readonly PayOSClient _payOS;

        private readonly ILogger<CreateNewPaymentOrder> _logger;


        public CreateNewPaymentOrder(PayOSClient payOSClient, ILogger<CreateNewPaymentOrder> logger)
        {
            _payOS = payOSClient;
            _logger = logger;
        }

        public async Task<string> Excute(OrderDTOInternal order)
        {
            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
                Amount = (long)order.TotalAmount,
                Description = "2HONDAICODON",
                ReturnUrl = "https://your-url.com",
                CancelUrl = "https://your-url.com"
            };

            var paymentLink = await _payOS.PaymentRequests.CreateAsync(paymentRequest);

            if (paymentLink.QrCode != null)
            {
                return paymentLink.QrCode;
            }
            else
            {
                return "Create Payment Fail";
            }
        }



    }
}
