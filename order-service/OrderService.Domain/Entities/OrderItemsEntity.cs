using order_service.OrderService.Domain.OjectValue;

namespace order_service.OrderService.Domain.Entities
{
    public class OrderItemsEntity
    {
        public Guid IdOrderItems { get; private set; }
        public Guid IdOrder { get; private set; }
        public Guid IdProduct { get; private set; }
        public string ProductName { get; private set; } = null!;

        public Guid? IdVariant { get; private set; }
        public string? VariantName { get; private set; }
        public Price UnitPrice { get; private set; }
        public QuantityItem Quantity { get; private set; }
        public Price TotalPrice { get; private set; }
        public string OptionsSnapshot { get; private set; } = string.Empty;


        internal OrderItemsEntity(Guid idOrderItems, Guid idOrder, Guid idProduct, string productName, Guid? idVariant, string? variantName, decimal unitPrice, int quantity, decimal totalPrice, string optionsSnapshot)
        {
            IdOrderItems = idOrderItems;
            IdOrder = idOrder;
            IdProduct = idProduct;
            ProductName = productName;
            IdVariant = idVariant;
            VariantName = variantName;
            UnitPrice = new Price(unitPrice);
            Quantity = new QuantityItem(quantity);
            TotalPrice = new Price(totalPrice);
            OptionsSnapshot = optionsSnapshot;
        }

        private OrderItemsEntity()
        {
        }

        public static OrderItemsEntity CreateOrderItems(Guid OrderId, Guid ProductId, string ProductName, Guid? VariantId, string? VariantName, decimal UnitPrice, int Quantity, decimal TotalPrice)
        {
            return new OrderItemsEntity
            {

                IdOrderItems = Guid.NewGuid(),
                IdOrder = OrderId,
                IdProduct = ProductId,
                ProductName = ProductName,
                IdVariant = VariantId,
                OptionsSnapshot = VariantName ?? string.Empty,
                Quantity = new QuantityItem(Quantity),
                VariantName = VariantName,
                UnitPrice = new Price(UnitPrice),
                TotalPrice = new Price(TotalPrice)

            };

        }



    }
}
