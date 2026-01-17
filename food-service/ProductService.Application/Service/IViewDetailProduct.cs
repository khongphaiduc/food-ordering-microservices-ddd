using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Service
{
    public interface IViewDetailProduct
    {
        Task<ProductDetailDTO> ExcuteAsync(Guid idProduct);
    }
}
