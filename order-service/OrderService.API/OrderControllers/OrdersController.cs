using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.OrderService.Application.DTOs;
using order_service.OrderService.Application.Services;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infrastructure.Models;
using System.Threading.Tasks;

namespace order_service.OrderService.API.OrderControllers
{
    [Route("api/orders")]
    [Authorize(AuthenticationSchemes = "AccessToken")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        private readonly ICreateNewOrder _order;
        private readonly IGetListOrderOfUser _getListOrder;
        private readonly IGetViewDetailOrder _viewDetailOrder;

        public OrdersController(ICreateNewOrder createNewOrder, IGetListOrderOfUser getListOrderOfUser, IGetViewDetailOrder getViewDetailOrder)
        {

            _order = createNewOrder;
            _getListOrder = getListOrderOfUser;
            _viewDetailOrder = getViewDetailOrder;
        }


        // create order 
        [HttpPost]
        public async Task<IActionResult> CreateNewOrder([FromHeader(Name = "Idempotency-Key")] Guid idempotencyKey, [FromBody] RequestPaymentCart request)
        {
            PaymentMethod methodPayment = (PaymentMethod)request.PaymentMethod;
            var result = await _order.Execute(idempotencyKey, request.IdCart, methodPayment, request.IdAddress);

            if (result.StatusCreateOrder)
            {
                return Ok(result);
            }

            if (result.ErrorCode == "INVENTORY_RESERVATION_FAILED")
            {
                return Conflict(result);
            }

            return BadRequest(result);
        }


        // xem danh sách order of user
        [HttpPost("histories")]
        public async Task<IActionResult> GetListOrders([FromBody] RequestGetListOrderWithPagination request)
        {
            var orders = await _getListOrder.GetListOrderForUser(request);
            return Ok(orders);
        }

        [HttpPost("detail")]
        public async Task<IActionResult> Index([FromBody] RequestViewOrderDetail request)
        {

            var orderDetail = await _viewDetailOrder.Execute(request);

            return Ok(orderDetail);
        }


    }
}
