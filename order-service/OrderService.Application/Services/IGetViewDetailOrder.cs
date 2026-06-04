using order_service.OrderService.Application.DTOs;

namespace order_service.OrderService.Application.Services
{
    public interface IGetViewDetailOrder
    {
        Task<ResponseViewDetailOrderDTO> Execute(RequestViewOrderDetail request);
    }
}
