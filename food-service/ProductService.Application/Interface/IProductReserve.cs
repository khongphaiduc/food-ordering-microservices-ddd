namespace food_service.ProductService.Application.Interface
{
    public interface IProductReserve
    {
        Task<bool> ReservecProduct(string productId, int quantity);
    }
}
