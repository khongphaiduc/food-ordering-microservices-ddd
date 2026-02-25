using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Domain.Enums;

namespace order_service.OrderService.Appilcation.Services
{
    public interface ICreateNewOrder
    {
        Task<string> Execute(Guid IdCart,PaymentMethod paymentMethod , Guid IdAddress);
    }
}
