using food_service.ProductService.Domain.Aggragate;

namespace food_service.ProductService.Domain.Interface
{
    public interface IProductRepository
    {
        Task<bool> AddProductAsync(ProductAggregate product);
        Task<ProductAggregate> GetProductByIdAsync(Guid productId);
        Task UpdateProductAsync(ProductAggregate product);


    }
}
