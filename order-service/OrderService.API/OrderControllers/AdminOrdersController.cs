using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.OrderService.Appilcation;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Interface;
using order_service.OrderService.Domain.Enums;
using System.Threading.Tasks;

namespace order_service.OrderService.API.OrderControllers
{
    [Route("api/orders")]
    [ApiController]
    public class AdminOrdersController : ControllerBase
    {
        private IGetListOrders _getListOrder;
        private readonly IUpdateStatusOrders _updateOrderStatus;

        public AdminOrdersController(IGetListOrders getListOrders, IUpdateStatusOrders updateStatusOrders)
        {
            _getListOrder = getListOrders;
            _updateOrderStatus = updateStatusOrders;
        }


        [HttpGet]
        public async Task<IActionResult> GetListOrder([FromQuery] RequestGetListOrder request)
        {
            var listOrder = await _getListOrder.Excute(request);
            return Ok(listOrder);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] RequestUpdateStatusOrder request)
        {
            var result = await _updateOrderStatus.Excute(request);
            return Ok(result);
        }

    }
}
