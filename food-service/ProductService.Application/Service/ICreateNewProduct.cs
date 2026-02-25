using food_service.ProductService.Application.DTOs.Request;

namespace food_service.ProductService.Application.Service
{
    public interface ICreateNewProduct
    {
        Task<bool> ExecuteAsync(CreateNewProducDTO request);
    }
}
