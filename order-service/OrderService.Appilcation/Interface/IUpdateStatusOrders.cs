namespace order_service.OrderService.Appilcation.Interface
{
    public interface IUpdateStatusOrders
    {
        Task<bool> Excute(RequestUpdateStatusOrder request);
    }
}
