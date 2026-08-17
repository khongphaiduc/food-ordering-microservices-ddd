using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Application.DTOs.DTOsInternal;
using order_service.OrderService.Domain.Enums;

namespace order_service.OrderService.Application.Services
{
    public interface ICreateNewOrder
    {
        Task<RequestCreateNewOrderAndPayment> Execute(Guid IdempotencyKey,Guid IdCart, PaymentMethod paymentMethod, Guid IdAddress);
    }


    public class RequestCreateNewOrderAndPayment
    {
        public bool StatusCreateOrder { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? ErrorCode { get; set; }

        public string? QRCodeString { get; set; }


    }

}
