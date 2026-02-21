namespace order_service.OrderService.Domain.Enums
{
    public enum OrderStatus
    {
        PENDING = 0,
        CONFIRMED = 1,
        PREPARING = 2,
        DELIVERING = 3,
        COMPLETED = 4,
        CANCELLED = 5
    }
}
