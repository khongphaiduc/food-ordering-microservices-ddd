using order_service.OrderService.Appilcation.DTOs;

namespace order_service.OrderService.Appilcation.Interface
{
    public interface IUpdateStatusOrders
    {
        Task<bool> Execute(RequestUpdateStatusOrder request);
    }
}
