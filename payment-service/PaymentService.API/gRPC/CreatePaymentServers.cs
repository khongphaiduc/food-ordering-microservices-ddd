using Grpc.Core;
using payment_service.PaymentService.Application.DTOs;
using payment_service.PaymentService.Application.Services;
using PaymentService.API.Proto;

namespace payment_service.PaymentService.API.gRPC
{
    public class CreatePaymentServers : PaymentInforGrpc.PaymentInforGrpcBase
    {
        private readonly ICreateNewPaymentOrder _createPaymentPayOS;

        public CreatePaymentServers(ICreateNewPaymentOrder createNewPaymentOrder)
        {
            _createPaymentPayOS = createNewPaymentOrder;
        }


        
        public override async Task<ResponseOrderQRCode> CreateNewPayment(RequestOrderCreatePayment request, ServerCallContext context)
        {

            var order = new OrderDTOInternal
            {
                OrderId = Guid.Parse(request.OrderId),
                Status = Domain.Enums.OrderStatus.PENDING,
                OrderCode = request.OrderCode,
                TotalAmount = (decimal)request.FinalAmount
            };

            var url = await _createPaymentPayOS.Excute(order);

            return new ResponseOrderQRCode
            {
                QRCodeString = url,
                StatusCreatePayment = "Success"
            };

        }
    }
}
