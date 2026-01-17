using food_service.productservice.infastructure.ProductDbContexts;
using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Service;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class GetListProduct : IGetListProduct
    {
        private readonly FoodProductsDbContext _db;

        public GetListProduct(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;
        }

        // search và phân trang
        public async Task<List<ProductDTO>> ExecuteAsync(RequestGetListProduct request)
        {
            var numberSkip = (request.PageIndex - 1) * request.PageSize;

            var query = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(p => p.Name.Contains(request.Keyword));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            query = query
                .OrderBy(p => p.Id)
                .Skip(numberSkip)
                .Take(request.PageSize);

            var listProduct = await query
                .Select(p => new ProductDTO
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Price = p.Price,
                    UrlImageMain = p.ProductImages.Where(img => img.IsMain).Select(img => img.ImageUrl).FirstOrDefault() ?? "https://img6.thuthuatphanmem.vn/uploads/2022/04/16/anh-rose-blackpink-cute-xinh_042754601.jpg",
                    IsAvailable = p.IsAvailable
                })
                .ToListAsync();

            return listProduct;
        }

    }
}
