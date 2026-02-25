using order_service.OrderService.Appilcation.DTOs;

namespace order_service.OrderService.Appilcation.Services
{
    public interface IGetViewDetailOreder
    {
        Task<ResponseViewDetailOrderDTO> Execute(RequestViewOrderDetail request);
    }
}
