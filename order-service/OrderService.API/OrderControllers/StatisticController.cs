using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Services;

namespace order_service.OrderService.API.OrderControllers
{
    [Authorize(AuthenticationSchemes = "AccessToken")]
    [Authorize(Roles ="Admin")]
    [Route("api/orders")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IGetNumberOfOrders _getNumberOrder;
        private readonly IRevenue _revenue;
        private readonly IGetPreFitOfMonths _getProfit;

        public StatisticController(IRevenue revenue, IGetNumberOfOrders getNumberOfOrders, IGetPreFitOfMonths getPreFitOfMonths)
        {
            _getNumberOrder = getNumberOfOrders;
            _revenue = revenue;
            _getProfit = getPreFitOfMonths;
        }

        [HttpGet("statistic")]
        public async Task<IActionResult> GetStatistic([FromQuery] GetRevenueDashboardRequest request)
        {
            var result = await _revenue.Excute(request);
            return Ok(result);
        }


        [HttpGet("statistic/order")]
        public async Task<IActionResult> GetStatisticOrder([FromQuery] GetOrderByMonthRequest request)
        {
            var result = await _getNumberOrder.Excute(request);
            return Ok(result);
        }

        [HttpGet("statistic/prefit")]
        public async Task<IActionResult> GetStatisticPrefit([FromQuery] RequestGetPrefitOfMonthDTO requets)
        {
            var result = await _getProfit.Excute(requets);
            return Ok(result);
        }
    }
}
