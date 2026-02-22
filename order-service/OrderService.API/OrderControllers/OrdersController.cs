using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Services;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infastructure.Models;
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
        private readonly IGetViewDetailOreder _viewDetailOrder;

        public OrdersController(ICreateNewOrder createNewOrder, IGetListOrderOfUser getListOrderOfUser, IGetViewDetailOreder getViewDetailOreder)
        {

            _order = createNewOrder;
            _getListOrder = getListOrderOfUser;
            _viewDetailOrder = getViewDetailOreder;
        }


        // tạo order 
        [HttpPost]
        public async Task<IActionResult> CreateNewOrder([FromBody] RequestPaymentCart request)
        {
            PaymentMethod methodPayment = (PaymentMethod)request.PaymentMethod;
            var QRCodeString = await _order.Excute(request.IdCart, methodPayment, request.IdAddress);
            return Ok(QRCodeString);
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

            var orderDetail = await _viewDetailOrder.Excute(request);

            return Ok(orderDetail);
        }


    }
}