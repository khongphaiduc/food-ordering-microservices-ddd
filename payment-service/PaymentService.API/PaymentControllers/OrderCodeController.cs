using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using payment_service.PaymentService.Application.Services;
using System.Net.WebSockets;

namespace payment_service.PaymentService.API.PaymentControllers
{
    [Route("api/payments")]
    [Authorize]
    [ApiController]
    public class OrderCodeController : ControllerBase
    {
        private readonly IGetORCodePayment _orderCode;

        public OrderCodeController(IGetORCodePayment getORCodePayment)
        {
            _orderCode = getORCodePayment;
        }

        [HttpGet("qrcode/{orderCode}")]
        public async Task<IActionResult> GetOrderCode([FromRoute] string orderCode, CancellationToken token)
        {
            var result = await _orderCode.GetORCodePaymentAsync(orderCode, token);
            return Ok(result);
        }
    }
}
