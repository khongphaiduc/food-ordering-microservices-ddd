using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Interface;
using order_service.OrderService.Domain.Enums;
using order_service.OrderService.Infastructure.Models;
using System.Linq.Expressions;

namespace order_service.OrderService.Infastructure.ServicesImplements
{
    public class GetListOrders : IGetListOrders
    {
        private readonly FoodOrderContext _db;

        public GetListOrders(FoodOrderContext foodOrderContext)
        {
            _db = foodOrderContext;
        }

        public async Task<ViewManagementOrder> Excute(RequestGetListOrder request)
        {
            var skipnumber = (request.CurrentPage - 1) * request.PageSize;

            var query = _db.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(request.OrderCode))
                query = query.Where(s => s.OrderCode == request.OrderCode);

            if (!string.IsNullOrEmpty(request.NameCustomer))
                query = query.Where(s => s.SnapshotNameCustomer.Contains(request.NameCustomer));

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                query = query.Where(s => s.SnapshotPhone.Contains(request.PhoneNumber));

            if (request.PaymentMethod != null)
                query = query.Where(s => s.PaymentMethod == request.PaymentMethod.ToString());

            if (request.OrderStatus != null)
                query = query.Where(s => s.OrderStatus == request.OrderStatus.ToString());

            if (request.FromDate != null)
                query = query.Where(s => s.CreatedAt >= request.FromDate);

            if (request.ToDate != null)
                query = query.Where(s => s.CreatedAt <= request.ToDate);


            var listOrder = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip(skipnumber)
                .Take(request.PageSize)
                .Select(s => new ViewOrderDTO
                {
                    IdOrder = s.Id,
                    NameCustomer = s.SnapshotNameCustomer ?? "Customer",
                    OrderCode = s.OrderCode,
                    PaymentMethod = Enum.Parse<PaymentMethod>(s.PaymentMethod!),
                    OrderStatusPayment = Enum.Parse<OrderStatusPayment>(s.Status),
                    CreateAt = s.CreatedAt,
                    orderStatus = Enum.Parse<OrderStatus>(s.OrderStatus),
                    TotalAmount = s.TotalAmount,
                })
                .ToListAsync();


            var counts = await _db.Orders
     .GroupBy(x => x.OrderStatus)
     .Select(g => new
     {
         Status = g.Key,
         Count = g.Count()
     })
     .ToListAsync();

            var result = new ViewManagementOrder
            {
                listOrderDTOs = listOrder,
                PreparingCount = counts.FirstOrDefault(x => x.Status == "PENDING")?.Count ?? 0,
                CancelledCount = counts.FirstOrDefault(x => x.Status == "CANCELLED")?.Count ?? 0,
                CompletedCount = counts.FirstOrDefault(x => x.Status == "COMPLETED")?.Count ?? 0,
                ConfirmationCount = counts.FirstOrDefault(x => x.Status == "CONFIRMED")?.Count ?? 0,
                DeliveringCount = counts.FirstOrDefault(x => x.Status == "DELIVERING")?.Count ?? 0
            };

            return result;
        }
    }
}
