
using CommunityToolkit.HighPerformance.Helpers;
using Elastic.Clients.Elasticsearch.Snapshot;
using food_service.ProductService.API.GlobalExceptions;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Domain.Aggregate;
using food_service.ProductService.Domain.Entities;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Domain.ValueObject;
using food_service.ProductService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using food_service.ProductService.Application;
using food_service.ProductService.Application.DTOs;
namespace food_service.ProductService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMinIOFood _minIo;
        private readonly FoodProductsDbContext _db;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IMinIOFood minIOFood, FoodProductsDbContext foodProductsDbContext,   ILogger<ProductRepository> logger)
        {
            _minIo = minIOFood;
            _db = foodProductsDbContext;
          
            _logger = logger;
        }

        public async Task<bool> AddProductAsync(ProductAggregate product)
        {
            _logger.LogInformation("Begin add new productRequest ");
            var transaction = await _db.Database.BeginTransactionAsync();
            try
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
                    ProductImages = product.ProductImagesEntities.Select(s => new ProductImage
                    {
                        Id = s.Id,
                        ProductId = s.ProductId,
                        ImageUrl = s.ImageUrl,
                        IsMain = s.IsMain,
                    }).ToList(),

                    ProductVariants = product.ProductVariantEntities.Select(q => new ProductVariant
                    {
                        Id = q.Id,
                        ProductId = q.ProductId,
                        Name = q.VariantName.Value,
                        CreatedAt = q.CreateAt,
                        ExtraPrice = q.ExtraPrice.Value,
                        IsActive = q.IsActive,
                        UpdatedAt = q.UpdateAt

                    }).ToList()
                };


                var productDTP = new ProductInternalDTO
                {
                    Id = producModel.Id,
                    Description = producModel.Description,
                    IdCategory = producModel.CategoryId,
                    Name = producModel.Name,
                    Price = producModel.Price,
                    UpdateAt = producModel.UpdatedAt,
                    CreateAt = producModel.CreatedAt,

                    productImageInternalDTOs = product.ProductImagesEntities.Select(s => new ProductImageInternalDTO
                    {
                        Id = s.Id,
                        IsMain = s.IsMain,
                        URLImage = s.ImageUrl,
                    }).ToList(),


                    productVariantInternalDTOs = product.ProductVariantEntities.Select(f => new ProductVariantInternalDTO
                    {
                        CreateAt = f.CreateAt,
                        Extra_Price = f.ExtraPrice.Value,
                        Name = f.VariantName.Value,
                        IdProduct = f.ProductId,
                        UpdateAt = f.UpdateAt,

                    }).ToList(),
                };


                await _db.Products.AddAsync(producModel);


                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("write new productRequest into OutBoxTable Successful");
                return true;
            }
            catch (Exception s)
            {

                await transaction.RollbackAsync();
                _logger.LogError($"Error in Repository Create new Product: {s}");
                return false;
            }
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

        // update  productRequest aggregate
        public async Task UpdateProductAsync(ProductAggregate productRequest)
        {

            _logger.LogInformation("Start update productRequest ");
            var productBase = await _db.Products.Include(s => s.ProductVariants).Include(s => s.ProductImages).FirstOrDefaultAsync(p => p.Id == productRequest.Id);


            if (productBase == null)
            {
                _logger.LogError($"Can not  find productRequest with Id {productRequest.Id}");
                throw new ProductNotFoundException($"ProductDTO with id {productRequest.Id} not found");
            }
            else
            {
                productBase.Name = productRequest.NameProduct.Value;
                productBase.Description = productRequest.Description;
                productBase.Price = productRequest.PriceProduct.Value;
                productBase.IsAvailable = productRequest.IsAvailable;
                productBase.UpdatedAt = productRequest.UpdatedAt;
                productBase.IsDeleted = productRequest.IsDeleted;


                // image 


                var imageDict = productBase.ProductImages.ToDictionary(s => s.Id);

                foreach (var imageReq in productRequest.ProductImagesEntities)
                {

                    if (!imageDict.TryGetValue(imageReq.Id, out var existingImage))
                    {

                        _db.ProductImages.Add(new ProductImage
                        {

                            Id = imageReq.Id,
                            ProductId = productBase.Id,
                            ImageUrl = imageReq.ImageUrl,
                            IsMain = imageReq.IsMain
                        });
                    }
                    else
                    {

                        existingImage.ImageUrl = imageReq.ImageUrl;
                        existingImage.IsMain = imageReq.IsMain;
                    }
                }

                //    Db có, reuqest không có  => xóa
                var listProductID = productRequest.ProductImagesEntities.Select(s => s.Id).ToHashSet();

                var _listImageRemove = productBase.ProductImages.Where(s => !listProductID.Contains(s.Id)).ToList();

                if (_listImageRemove.Any()) _db.RemoveRange(_listImageRemove);





                // variant 
                var variantDict = productBase.ProductVariants.ToDictionary(v => v.Id);


                foreach (var variantItem in productRequest.ProductVariantEntities)
                {
                    if (!variantDict.TryGetValue(variantItem.Id, out var variantEntity))
                    {

                        _db.ProductVariants.Add(new ProductVariant
                        {
                            Id = variantItem.Id,
                            ProductId = variantItem.ProductId,
                            Name = variantItem.VariantName.Value,
                            ExtraPrice = variantItem.ExtraPrice.Value,
                            IsActive = variantItem.IsActive,
                            CreatedAt = variantItem.CreateAt,
                            UpdatedAt = variantItem.UpdateAt
                        });
                    }
                    else
                    {

                        if (variantItem.ExtraPrice != null) variantEntity.ExtraPrice = variantItem.ExtraPrice.Value;


                        if (variantItem.VariantName != null) variantEntity.Name = variantItem.VariantName.Value;

                        variantEntity.IsActive = variantItem.IsActive;
                        variantEntity.UpdatedAt = variantItem.UpdateAt;
                    }
                }


                //remove variant 


                var variantIds = productRequest.ProductVariantEntities.Select(s => s.Id).ToHashSet();

                var listVariantRemove = productBase.ProductVariants.Where(v => !variantIds.Contains(v.Id)).ToList();

                if (listVariantRemove.Any()) _db.RemoveRange(listVariantRemove);



                var transaction = await _db.Database.BeginTransactionAsync();
                try
                {


                    var productDTP = new ProductInternalDTO
                    {
                        Id = productRequest.Id,
                        Description = productRequest.Description,
                        IdCategory = productRequest.CategoryId,
                        Name = productRequest.NameProduct.Value,
                        Price = productRequest.PriceProduct.Value,
                        UpdateAt = productRequest.UpdatedAt,
                        CreateAt = productRequest.CreatedAt,

                        productImageInternalDTOs = productRequest.ProductImagesEntities.Select(s => new ProductImageInternalDTO
                        {
                            Id = s.Id,
                            IsMain = s.IsMain,
                            URLImage = s.ImageUrl,
                        }).ToList(),


                        productVariantInternalDTOs = productRequest.ProductVariantEntities.Select(f => new ProductVariantInternalDTO
                        {
                            CreateAt = f.CreateAt,
                            Extra_Price = f.ExtraPrice.Value,
                            Name = f.VariantName.Value,
                            IdProduct = f.ProductId,
                            UpdateAt = f.UpdateAt,

                        }).ToList(),
                    };


                    if (productDTP.productImageInternalDTOs?.Any() == true)
                    {
                        var tasks = productDTP.productImageInternalDTOs
                            .Select(async image =>
                            {
                                image.URLImage = await _minIo.GetUrlImage("images", image.URLImage);
                            });

                        await Task.WhenAll(tasks);
                    }

                    var result = await _db.SaveChangesAsync() > 0;

                    await transaction.CommitAsync();
                    if (result)
                    {
                        _logger.LogInformation($"update productRequest id  {productRequest.Id} Successful");
                    }
                    else
                    {
                        _logger.LogInformation($"update productRequest id  {productRequest.Id} fail");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error while updating product");
                    throw;
                }

            }

        }
    }


}
/*   note
 Các tiêu chí l?a ch?n  Collections
   + N?u mu?n l?y và c?p nh?t ,   xóa thì => Directory
   + N?u mu?n ch? c?n ki?m tra s? t?n t?i ( Contain() ) => HashSet
 */
