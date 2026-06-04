using order_service.OrderService.Application.DTOs;

namespace order_service.OrderService.Application.Interface
{
    public interface IGetListOrders
    {
        Task<ViewManagementOrder> Execute(RequestGetListOrder request);
    }
}
