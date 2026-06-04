using food_service.ProductService.Infrastructure.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using productService.API.Protos;

namespace food_service.ProductService.API.gRPC
{
    public class LoadFullProduct : ProductListGrpc.ProductListGrpcBase
    {
        private readonly FoodProductsDbContext _db;

        public LoadFullProduct(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;
        }

        public override async Task<ResponseProductList> GetListProductsFood(none request, ServerCallContext context)
        {
            var s = new ResponseProductList();
            var list = await _db.Products.Select(s => new global::productService.API.Protos.Product
            {
                IdProduct = s.Id.ToString(),
                NameProduct = s.Name

            }).ToListAsync();

            s.Payload.AddRange(list);
            return s;
        }
    }
}
