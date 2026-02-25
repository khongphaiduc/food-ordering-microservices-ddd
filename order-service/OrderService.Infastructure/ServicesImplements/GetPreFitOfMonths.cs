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
    public class GetPreFitOfMonths : IGetPreFitOfMonths
    {
        private readonly FoodOrderContext _db;

        public GetPreFitOfMonths(FoodOrderContext foodOrderContext)
        {
            _db = foodOrderContext;
        }

        public async Task<GetPreFitOfMonthDTO> Execute(RequestGetPrefitOfMonthDTO requets)
        {
           
            if (requets.Year < 1 || requets.Month < 1 || requets.Month > 12)
            {
                throw new ArgumentException("Invalid year or month");
            }

            var firstDayOfMonth = new DateTime(requets.Year, requets.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            
            var ordersInMonth = await _db.Orders
                .Where(o => o.CreatedAt >= firstDayOfMonth
                    && o.CreatedAt <= lastDayOfMonth.AddDays(1)
                    && o.Status == "PAID"
                    && o.OrderStatus == "COMPLETED")
                .AsNoTracking()
                .ToListAsync();

          
            var profitByDay = ordersInMonth
                .GroupBy(o => o.CreatedAt.Day)
                .OrderBy(g => g.Key)
                .Select(g => new PrefitByDayDTO
                {
                    Day = g.Key,
                    Amount = (int)g.Sum(o => o.FinalAmount)
                })
                .ToList();

          
            var allDaysInMonth = GetAllDaysInMonth(requets.Year, requets.Month);
            var result = new GetPreFitOfMonthDTO
            {
                Year = requets.Year,
                Month = requets.Month,
                Data = allDaysInMonth
                    .Select(day => new PrefitByDayDTO
                    {
                        Day = day,
                        Amount = profitByDay.FirstOrDefault(p => p.Day == day)?.Amount ?? 0
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