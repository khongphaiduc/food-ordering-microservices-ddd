using order_service.OrderService.Appilcation.DTOs;

namespace order_service.OrderService.Appilcation.Interface
{
    public interface IStaffViewDetailOrders
    {
        Task<StaffViewDetailOrderDTO> Excute(Guid IdOrder);
    }
}
