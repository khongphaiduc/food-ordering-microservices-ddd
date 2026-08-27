using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using order_service.OrderService.API.Proto;
using order_service.OrderService.Infrastructure.Models;

namespace order_service.OrderService.API.gRPC
{
    public class GetOrderByOrderCode : OrderByOrderCodeGrpc.OrderByOrderCodeGrpcBase
    {
        private readonly FoodOrderContext _db;
        private readonly ILogger<GetOrderByOrderCode> _logger;

        public GetOrderByOrderCode(ILogger<GetOrderByOrderCode> logger, FoodOrderContext foodOrderContext)
        {
            _db = foodOrderContext;
            _logger = logger;
        }

        public override async Task<ResponseOrderByOrderCode> ViewOrderDetailByOrderCode(RequestOrderByOrderCode request, ServerCallContext context)
        {
            try
            {
                var order = await _db.Orders.Include(t => t.OrderItems).FirstOrDefaultAsync(s => s.OrderCode == request.OrderCode);

                if (order != null)
                {

                    var orderIterm = order.OrderItems.Select(s => new global::order_service.OrderService.API.Proto.OrderItemByOrderCode
                    {
                        ProductId = s.ProductId.ToString(),
                        Quantity = s.Quantity

                    }).ToList();

                    var resp = new ResponseOrderByOrderCode
                    {
                        OrderCode = request.OrderCode,
                        Amount = (long)order.FinalAmount,
                        PaymentMenthod = order.PaymentMethod,
                        OrderItems = { orderIterm },
                        DateTime = order.CreatedAt.ToString()
                    };

                    return resp;
                }
                return new ResponseOrderByOrderCode();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Bug :{ex.Message}");
                throw;
            }
        }
    }
}
