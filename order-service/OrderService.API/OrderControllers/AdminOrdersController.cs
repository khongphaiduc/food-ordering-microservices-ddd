using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Interface;
using order_service.OrderService.Appilcation.Services;
using order_service.OrderService.Domain.Enums;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace order_service.OrderService.API.OrderControllers
{
    [Route("api/orders")]
    [ApiController]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IStaffViewDetailOrders _staffViewOrderDetail;
        private readonly IGetListOrders _getListOrder;
        private readonly IUpdateStatusOrders _updateOrderStatus;


        public AdminOrdersController(IGetListOrders getListOrders, IUpdateStatusOrders updateStatusOrders, IStaffViewDetailOrders staffViewDetailOrders)
        {
            _staffViewOrderDetail = staffViewDetailOrders;
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

        [HttpGet("{idOrder}")]
        public async Task<IActionResult> ViewOrderDetail([FromRoute] Guid idOrder)
        {
            var orderDetail = await _staffViewOrderDetail.Excute(idOrder);
            return Ok(orderDetail);
        }

    }
}
