using search_service.Models;
using search_service.SearchService.Application.IndexModel;
using search_service.SearchService.Application.Interface;

namespace search_service.SearchService.Infrastructure.ImplementServices
{
    public class GetListProduct : IGetListProduct
    {
        private readonly FoodProductsDbContext _db;

        public GetListProduct(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;

        }
        public Task<List<ProductDoc>> ExcuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
