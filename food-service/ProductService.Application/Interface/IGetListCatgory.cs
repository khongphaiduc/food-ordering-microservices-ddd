using food_service.ProductService.Application.DTOs.Response;

namespace food_service.ProductService.Application.Interface
{
    public interface IGetListCatgory
    {
        Task<List<CatgoryDTO>> Excute();
    }
}
