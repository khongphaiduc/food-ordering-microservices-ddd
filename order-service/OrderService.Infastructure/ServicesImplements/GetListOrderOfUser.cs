using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Services;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infastructure.Models;

namespace order_service.OrderService.Infastructure.ServicesImplements
{
    public class GetListOrderOfUser : IGetListOrderOfUser
    {
        private readonly FoodOrderContext _db;

        public GetListOrderOfUser(FoodOrderContext foodOrderContext)
        {
            _db = foodOrderContext;
        }

        public async Task<OrderHistoryPagination> GetListOrderForUser(RequestGetListOrderWithPagination request)
        {
            int index = request.PageIndex;
            int takeNumber = 7;
            var skipNumber = (index - 1) * takeNumber;

            var order = await _db.Orders
      .Where(o => o.UserId == request.IdUser)
      .OrderByDescending(s => s.CreatedAt)
      .Skip(skipNumber)
      .Take(takeNumber)
      .Select(s => new ResponseListOrderOfUser
      {
          IDOrder = s.Id,
          TotalPrice = s.FinalAmount,
          CreateAt = s.CreatedAt,
          OrderCode = s.OrderCode,
          OrderStatusPayment = Enum.Parse<OrderStatusPayment>(s.Status),
          OrderStatus = Enum.Parse<OrderStatus>(s.OrderStatus),
          PaymentMethod = Enum.Parse<PaymentMethod>(s.PaymentMethod!)
      })
      .ToListAsync();

            return new OrderHistoryPagination
            {
                orderHistory = order,
                TotalPages = (int)Math.Ceiling((double)_db.Orders.Where(o => o.UserId == request.IdUser).Count() / takeNumber)
            };
        }
    }
}
