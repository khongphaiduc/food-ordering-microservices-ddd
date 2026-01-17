using food_service.productservice.infastructure.Models;
using food_service.productservice.infastructure.ProductDbContexts;
using food_service.ProductService.API.GlobalExceptions;
using food_service.ProductService.Domain.Aggragate;
using food_service.ProductService.Domain.Entities;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Domain.ValueOject;
using Microsoft.EntityFrameworkCore;

namespace food_service.ProductService.Infastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly FoodProductsDbContext _db;

        public ProductRepository(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;
        }

        public async Task<bool> AddProductAsync(ProductAggregate product)
        {

            var producModel = new Product
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.NameProduct.Value,
                Description = product.Description,
                Price = product.PriceProduct.Value,
                IsAvailable = product.IsAvailable,
                IsDeleted = product.IsDeleted,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
            };

            await _db.Products.AddAsync(producModel);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<ProductAggregate> GetProductByIdAsync(Guid productId)
        {
            var product = await _db.Products.Include(s => s.ProductVariants).Include(s => s.ProductImages).FirstOrDefaultAsync(s => s.Id == productId);
            if (product != null)
            {
                var productAggregate = new ProductAggregate(
                 product.Id,
                 product.CategoryId,
                 new Name(product.Name),
                 new Price(product.Price),
                 product.Description,
                 product.IsAvailable,
                 product.IsDeleted,
                 product.CreatedAt,
                 product.UpdatedAt,
                 product.ProductImages.Select(s => new ProductImagesEntity(s.Id, s.ProductId, s.ImageUrl, s.IsMain)).ToList(),
                 product.ProductVariants.Select(pv => new ProductVariantEntity(pv.Id, pv.ProductId, new Name(pv.Name), new Price(pv.ExtraPrice), pv.IsActive, pv.CreatedAt, pv.UpdatedAt)).ToList()
                );
                return productAggregate;
            }
            else
            {
                throw new ProductNotFoundException($"ProductDTO with id {productId} not found");
            }
        }

        // cập nhật product aggregate
        public async Task UpdateProductAsync(ProductAggregate product)
        {


            var productBase = await _db.Products.Include(s => s.ProductVariants).Include(s => s.ProductImages).FirstOrDefaultAsync(p => p.Id == product.Id);


            if (productBase == null)
            {
                throw new ProductNotFoundException($"ProductDTO with id {product.Id} not found");
            }
            else
            {
                productBase.Name = product.NameProduct.Value;
                productBase.Description = product.Description;
                productBase.Price = product.PriceProduct.Value;
                productBase.IsAvailable = product.IsAvailable;
                productBase.UpdatedAt = DateTime.UtcNow;
                productBase.IsDeleted = product.IsDeleted;


                foreach (var image in product.ProductImagesEntities)
                {
                    if (!productBase.ProductImages.Any(s => s.Id == image.Id))
                    {
                        productBase.ProductImages.Add(new ProductImage()
                        {
                            Id = image.Id,
                            ProductId = image.ProductId,
                            ImageUrl = image.ImageUrl,
                            IsMain = image.IsMain
                        });
                    }
                    else
                    {
                        var imageToUpdate = productBase.ProductImages.First(s => s.Id == image.Id);
                        imageToUpdate.ImageUrl = image.ImageUrl;
                        imageToUpdate.IsMain = image.IsMain;
                    }
                }


                foreach (var variant in product.ProductVariantEntities)
                {
                    if (!productBase.ProductVariants.Any(s => s.Id == variant.Id))
                    {
                        productBase.ProductVariants.Add(new ProductVariant()
                        {
                            Id = variant.Id,
                            ProductId = variant.ProductId,
                            Name = variant.VariantName.Value,
                            ExtraPrice = variant.ExtraPrice.Value,
                            IsActive = variant.IsActive,
                            CreatedAt = variant.CreateAt,
                            UpdatedAt = variant.UpdateAt
                        });
                    }
                    else
                    {
                        var variantToUpdate = productBase.ProductVariants.First(s => s.Id == variant.Id);
                        variantToUpdate.Name = variant.VariantName.Value;
                        variantToUpdate.ExtraPrice = variant.ExtraPrice.Value;
                        variantToUpdate.IsActive = variant.IsActive;
                        variantToUpdate.UpdatedAt = DateTime.UtcNow;
                    }
                }
                await _db.SaveChangesAsync();
            }

        }
    }
}
