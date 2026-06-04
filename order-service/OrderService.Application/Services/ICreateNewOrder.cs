using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Domain.Enums;

namespace order_service.OrderService.Application.Services
{
    public interface ICreateNewOrder
    {
        Task<string> Execute(Guid IdCart,PaymentMethod paymentMethod , Guid IdAddress);
    }
}
