using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Services;
using order_service.OrderService.Infastructure.Models;

namespace order_service.OrderService.Infastructure.ServicesImplements
{
    public class GetNumberOfOrders : IGetNumberOfOrders
    {
        private readonly FoodOrderContext _db;

        public GetNumberOfOrders(FoodOrderContext foodOrderContext)
        {
            _db = foodOrderContext;
        }

        public async Task<RequestGetNumberOrderOfMonthDTO> Execute(GetOrderByMonthRequest request)
        {
          
            if (request.Year < 1 || request.Month < 1 || request.Month > 12)
            {
                throw new ArgumentException("Invalid year or month");
            }

            
            var firstDayOfMonth = new DateTime(request.Year, request.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

           
            var ordersInMonth = await _db.Orders
                .Where(o => o.CreatedAt >= firstDayOfMonth && o.CreatedAt <= lastDayOfMonth.AddDays(1))
                .AsNoTracking()
                .ToListAsync();

           
            var ordersByDay = ordersInMonth
                .GroupBy(o => o.CreatedAt.Day)
                .OrderBy(g => g.Key)
                .Select(g => new OrderByDayDTO
                {
                    Day = g.Key,
                    OrderCount = g.Count()
                })
                .ToList();

           
            var allDaysInMonth = GetAllDaysInMonth(request.Year, request.Month);
            var result = new RequestGetNumberOrderOfMonthDTO
            {
                Year = request.Year,
                Month = request.Month,
                Data = allDaysInMonth
                    .Select(day => new OrderByDayDTO
                    {
                        Day = day,
                        OrderCount = ordersByDay.FirstOrDefault(o => o.Day == day)?.OrderCount ?? 0
                    })
                    .ToList()
            };

            return result;
        }

        /// <summary>
        /// Get all days in the specified month
        /// </summary>
        private List<int> GetAllDaysInMonth(int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            return Enumerable.Range(1, daysInMonth).ToList();
        }
    }
}