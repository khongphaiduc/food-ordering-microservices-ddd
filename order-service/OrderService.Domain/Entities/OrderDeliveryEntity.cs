using order_service.OrderService.Domain.ObjectValue;

namespace order_service.OrderService.Domain.Entities
{
    public class OrderDeliveryEntity
    {
        public OrderDeliveryEntity()
        {
        }

        public Guid OrderID { get; private set; }

        public string ReciverName { get; private set; } = string.Empty;

        public Phone PhoneNumer { get; private set; }

        public Address Address { get; private set; }

        public string Note { get; private set; } = string.Empty;

        public DateTime EstimatedTime { get; private set; }

        internal OrderDeliveryEntity(Guid orderID, string reciverName, string phone, string address, string note, DateTime estimatedTime)
        {
            OrderID = orderID;
            ReciverName = reciverName;
            PhoneNumer = new Phone(phone);
            Address = new Address(address);
            Note = note;
            EstimatedTime = estimatedTime;
        }

        public static OrderDeliveryEntity CreateNewOrderDelivery(Guid orderID, string? reciverName, string? phone, string? address, string? note, DateTime estimatedTime)
        {
            return new OrderDeliveryEntity
            {
                Address = new Address(address),
                Note = note,
                EstimatedTime = estimatedTime,
                OrderID = orderID,
                PhoneNumer = new Phone(phone),
                ReciverName = reciverName

            };
        }

        public OrderDeliveryEntity(Guid orderID, string reciverName, Phone phoneNumer, Address address, string note, DateTime estimatedTime)
        {
            OrderID = orderID;
            ReciverName = reciverName;
            PhoneNumer = phoneNumer;
            Address = address;
            Note = note;
            EstimatedTime = estimatedTime;
        }
    }
}
