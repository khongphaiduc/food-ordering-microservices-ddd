using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using order_service.OrderService.Appilcation.DTOs;
using order_service.OrderService.Appilcation.Services;
using order_service.OrderService.Infastructure.Models;

public class Revenue : IRevenue
{
    private readonly FoodOrderContext _db;

    public Revenue(FoodOrderContext foodOrderContext)
    {
        _db = foodOrderContext;
    }

    public async Task<RevenueDashboardResponse> Excute(GetRevenueDashboardRequest request)
    {
        // Validate input
        if (request.FromDate > request.ToDate)
        {
            throw new ArgumentException("FromDate must be less than or equal to ToDate");
        }

        // Get current period revenue
        var currentPeriodOrders = await GetOrdersByDateRange(request.FromDate, request.ToDate);
        var currentRevenue = currentPeriodOrders
            .Where(o => o.Status == "PAID" && (o.OrderStatus == "COMPLETED" || o.OrderStatus == "DELIVERING"))
            .Sum(o => o.FinalAmount);
        var currentOrderCount = currentPeriodOrders.Count();
        var currentCompletedOrders = currentPeriodOrders
            .Count(o => o.OrderStatus == "COMPLETED" && o.Status == "PAID");

        // Get compare period revenue based on CompareType
        var compareRevenue = 0m;
        var compareOrderCount = 0;
        var compareCompletedOrders = 0;

        if (request.CompareType != RevenueCompareType.None)
        {
            var (compareFromDate, compareToDate) = GetCompareDateRange(request.FromDate, request.ToDate, request.CompareType);

            var comparePeriodOrders = await GetOrdersByDateRange(compareFromDate, compareToDate);
            compareRevenue = comparePeriodOrders
                .Where(o => o.Status == "PAID" && (o.OrderStatus == "COMPLETED" || o.OrderStatus == "DELIVERING"))
                .Sum(o => o.FinalAmount);
            compareOrderCount = comparePeriodOrders.Count();
            compareCompletedOrders = comparePeriodOrders
                .Count(o => o.OrderStatus == "COMPLETED" && o.Status == "PAID");
        }

        // Calculate differences and growth percentage
        var revenueDifference = currentRevenue - compareRevenue;
        var revenueGrowthPercent = compareRevenue == 0
            ? (currentRevenue > 0 ? 100 : 0)
            : Math.Round((revenueDifference / compareRevenue) * 100, 2);

        // Calculate completion percentage
        var percentCompletion = currentOrderCount == 0
            ? 0
            : Math.Round((decimal)currentCompletedOrders / currentOrderCount * 100, 2);

        return new RevenueDashboardResponse
        {
            CurrentRevenue = currentRevenue,
            CompareRevenue = compareRevenue,
            RevenueDifference = revenueDifference,
            RevenueGrowthPercent = revenueGrowthPercent,
            CurrentOrderCount = currentOrderCount,
            CompareOrderCount = compareOrderCount,
            PercentComplation = percentCompletion
        };
    }

    /// <summary>
    /// Lấy danh sách đơn hàng trong khoảng thời gian
    /// </summary>
    private async Task<IQueryable<Order>> GetOrdersByDateRange(DateTime fromDate, DateTime toDate)
    {
        return _db.Orders
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate.AddDays(1))
            .AsQueryable();
    }

    /// <summary>
    /// Xác định khoảng thời gian so sánh dựa trên CompareType
    /// </summary>
    private (DateTime fromDate, DateTime toDate) GetCompareDateRange(
        DateTime currentFromDate,
        DateTime currentToDate,
        RevenueCompareType compareType)
    {
        var daysDifference = (currentToDate - currentFromDate).Days + 1;

        return compareType switch
        {
            RevenueCompareType.Yesterday =>
                (currentFromDate.AddDays(-1), currentToDate.AddDays(-1)),

            RevenueCompareType.LastWeek =>
                (currentFromDate.AddDays(-7), currentToDate.AddDays(-7)),

            RevenueCompareType.LastMonth =>
                (currentFromDate.AddMonths(-1), currentToDate.AddMonths(-1)),

            _ => throw new ArgumentException($"Invalid CompareType: {compareType}")
        };
    }
}