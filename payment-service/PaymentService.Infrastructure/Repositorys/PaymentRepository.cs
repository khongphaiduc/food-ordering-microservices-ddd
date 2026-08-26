using Microsoft.EntityFrameworkCore;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Domain.Enums;
using payment_service.PaymentService.Infrastructure.Models;

namespace payment_service.PaymentService.Infrastructure.Repositorys
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly FoodPaymentContext _db;

        public PaymentRepository(FoodPaymentContext foodPaymentContext)
        {
            _db = foodPaymentContext;
        }

        public async Task<bool> CreatePayment(CreatePaymentPayload request)
        {
            var id = Guid.NewGuid();

            _db.Payments.Add(new Payment
            {
                Id = id,
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow,
                Currency = request.Currency,
                OrderCode = request.OrderCode,
                PaymentMethod = request.PaymentMethods.ToString(),
                Status = "Pending",
                UserId = request.UserId,
            });

            _db.PaymentTransactions.Add(new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                PaymentId = id,
                CreatedAt = DateTime.UtcNow,
                OrderQRCode = request.QRCode,
                Status = PaymentStatus.Pending.ToString(),       
            });

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStatusPayment(string OrderCode, PaymentStatus status)
        {
            return await _db.Database.ExecuteSqlInterpolatedAsync($"""
        UPDATE [dbo].[Payments]
        SET 
            [Status] = {status.ToString()},
            [UpdatedAt] = GETUTCDATE()
        WHERE [OrderCode] = {OrderCode}
          AND [Status] = 'Pending'
        """) > 0;
        }
    }
}
