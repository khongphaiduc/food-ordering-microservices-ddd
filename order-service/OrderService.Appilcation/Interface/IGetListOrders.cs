using order_service.OrderService.Appilcation.DTOs;

namespace order_service.OrderService.Appilcation.Interface
{
    public interface IGetListOrders
    {
        Task<ViewManagementOrder> Execute(RequestGetListOrder request);
    }
}
