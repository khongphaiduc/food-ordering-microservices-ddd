using order_service.OrderService.Domain.Enums;

namespace Foodly.Contracts.Events
{
    public class CreatedOrderEvent
    {
        public Guid IdOrder { get; set; }

        public string OrderMethodPayment { get; set; }  = PaymentMethod.PayOS.ToString();
        public Guid IdUser { get; set; }
    }
}
