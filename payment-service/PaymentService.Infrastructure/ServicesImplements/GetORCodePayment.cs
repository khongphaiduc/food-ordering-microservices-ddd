using Microsoft.EntityFrameworkCore;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Domain.Enums;
using payment_service.PaymentService.Infrastructure.Models;
using System.Net.WebSockets;

namespace payment_service.PaymentService.Infrastructure.ServicesImplements
{
    public class GetORCodePayment : IGetORCodePayment
    {
        private readonly FoodPaymentContext _db;

        public GetORCodePayment(FoodPaymentContext foodPaymentContext)
        {
            _db = foodPaymentContext;
        }

        public async Task<ORCodeDto> GetORCodePaymentAsync(string code, CancellationToken token)
        {

            var payment = await _db.Payments.Include(t => t.PaymentTransactions).FirstOrDefaultAsync(s => s.OrderCode == code && s.Status == PaymentStatus.Pending.ToString(), token);

            if (payment != null)
            {
                return new ORCodeDto
                {
                    IdPayment = payment.Id,
                    QRCode = payment.PaymentTransactions.Select(s => s.OrderQRCode).FirstOrDefault() ?? "None"
                };
            }
            return new ORCodeDto();
        }
    }
}
