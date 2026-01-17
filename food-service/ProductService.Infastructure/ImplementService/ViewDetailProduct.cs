using food_service.productservice.infastructure.ProductDbContexts;
using food_service.ProductService.API.GlobalExceptions;
using food_service.ProductService.Application.DTOs.Response;
using food_service.ProductService.Application.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class ViewDetailProduct : IViewDetailProduct
    {
        private readonly FoodProductsDbContext _db;

        public ViewDetailProduct(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;
        }

        public async Task<ProductDetailDTO> ExcuteAsync(Guid idProduct)
        {

            var product = await _db.Products.Where(s => s.Id == idProduct).Select(s => new ProductDetailDTO
            {

                IdProduct = s.Id,
                Description = s.Description ?? "None",
                Price = s.Price,
                productImageDTOs = s.ProductImages.Where(t => t.ProductId == s.Id).Select(c => new ProductImageDTO
                {
                    IdImage = c.Id,
                    UrlImage = c.ImageUrl,
                }).ToList(),

                productVariantDTOs = s.ProductVariants.Where(b => b.ProductId == s.Id).Select(g => new ProductVariantDTO
                {
                    IdVariant = g.Id.ToString(),
                    Name = g.Name,
                    ExtraPrice = g.ExtraPrice,

                }).ToList(),


            }).FirstOrDefaultAsync();

            if (product == null)
            {
                return null;
            }

            return product;
        }
    }
}
