using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Service
{
    public interface ISearchProducts
    {
        Task<List<ProductDTO>> SearchProductsAsync(string query,int index, CancellationToken cancellationToken);
    }
}
