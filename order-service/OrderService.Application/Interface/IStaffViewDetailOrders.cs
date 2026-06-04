using order_service.OrderService.Application.DTOs;

namespace order_service.OrderService.Application.Interface
{
    public interface IStaffViewDetailOrders
    {
        Task<StaffViewDetailOrderDTO> Execute(Guid IdOrder);
    }
}
