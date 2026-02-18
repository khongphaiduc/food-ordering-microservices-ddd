using CommunityToolkit.HighPerformance.Helpers;
using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Aggragate;
using food_service.ProductService.Domain.Entities;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Domain.ValueOject;
using food_service.ProductService.Infastructure.Models;
using Microsoft.EntityFrameworkCore;
using Minio;
using RabbitMQ.Client;
using static System.Net.Mime.MediaTypeNames;

namespace food_service.ProductService.Infastructure.ImplementService
{
    public class UpdateProduct : IUpdateProduct
    {
        private readonly IProductRepository _product;
        private readonly FoodProductsDbContext _db;
        private readonly IMinIOFood _minIO;
        private readonly ILogger<UpdateProduct> _logger;

        public UpdateProduct(IProductRepository productRepository, FoodProductsDbContext db, IMinIOFood minIOFood, ILogger<UpdateProduct> logger)
        {
            _product = productRepository;
            _db = db;
            _minIO = minIOFood;
            _logger = logger;
        }


        // update image product 
        public async Task Excute(UpdateProductDTO productRequest)
        {

            var product = await _db.Products.Include(s => s.ProductVariants).Include(s => s.ProductImages).Where(s => s.Id == productRequest.IdProduct).FirstOrDefaultAsync();



            if (product == null)
            {
                _logger.LogError($"Product dfnasdfasbfdkjasbfdasdf with ID : {productRequest.IdProduct} is not found");
                return;
            }


            _logger.LogInformation($"Product tồn tại với id {product.Id}");

            var listImage = product.ProductImages.Select(s => new ProductImagesEntity(s.Id, s.ProductId, s.ImageUrl, s.IsMain)).ToList();

            var listVariant = product.ProductVariants.Select(s => new ProductVariantEntity(s.Id, s.ProductId, new Name(s.Name), new Price(s.ExtraPrice), s.IsActive, s.CreatedAt, s.UpdatedAt)).ToList();

            var productAggregate = new ProductAggregate(product.CategoryId, product.Id, new(product.Name),
                new Domain.ValueOject.Price(product.Price), product.Description, product.IsAvailable,
                product.IsDeleted, product.CreatedAt, product.UpdatedAt, listImage, listVariant);


            if (productRequest.Description != null) productAggregate.ChangeDescription(productRequest.Description);

            if (productRequest.Price != null) productAggregate.ChangePrice(new Price(productRequest.Price.Value));

            if (productRequest.Name != null) productAggregate.ChangeName(new Name(productRequest.Name));



            // add new images
            if (productRequest.AddnewImagesProducts != null && productRequest.AddnewImagesProducts.Any())
            {
                foreach (var image in productRequest.AddnewImagesProducts)
                {
                    var nameImage = await _minIO.UploadAsync(image.images);
                    productAggregate.AddNewImage(ProductImagesEntity.CreateNewImage(productAggregate.Id, nameImage, image.IsMain));
                }
            }

            // add main image 
            if (productRequest.AddMainImage != null)
            {
                var oldMainImage = productAggregate.ProductImagesEntities.Where(s => s.IsMain).FirstOrDefault();
                if (oldMainImage != null) oldMainImage.UnsetAsMainImage();
                var nameImage = await _minIO.UploadAsync(productRequest.AddMainImage.images);
                productAggregate.AddNewImage(ProductImagesEntity.CreateNewImage(productAggregate.Id, nameImage, true));

            }


            // remove image 
            if (productRequest.DeleteImage != null && productRequest.DeleteImage.Any())
            {
                foreach (var imageId in productRequest.DeleteImage)
                {
                    var image = productAggregate.ProductImagesEntities.FirstOrDefault(x => x.Id == imageId);

                    if (image != null)
                    {
                        var imageEntity = productAggregate.ProductImagesEntities.FirstOrDefault(s => s.Id == image.Id);
                        if (imageEntity != null)
                        {
                            productAggregate.DeleteImage(imageEntity);
                        }
                    }

                    if (image != null)  // remove image in minio
                    {
                        await _minIO.DeleteAsync(image.ImageUrl);
                    }
                }


                var checkMainImage = productAggregate.ProductImagesEntities.Where(s => s.IsMain).FirstOrDefault();
                if (checkMainImage == null)
                {
                    var SetMainImage = productAggregate.ProductImagesEntities.FirstOrDefault();
                    if (SetMainImage != null) { SetMainImage.SetAsMainImage(); }
                }

            }


            // add new variant 
            if (productRequest.AddNewVariantDTOs != null && productRequest.AddNewVariantDTOs.Any())
            {
                foreach (var variantItem in productRequest.AddNewVariantDTOs)
                {
                    productAggregate.AddNewVariant(ProductVariantEntity.CreateNewVariant(product.Id, new Name(variantItem.Name), new Price(variantItem.ExtraPrice), variantItem.IsMain));
                }
            }

            // update variant 
            if (productRequest.UpdateVariant != null && productRequest.UpdateVariant.Any())
            {
                foreach (var item in productRequest.UpdateVariant)
                {
                    var variantUpdate = productAggregate.ProductVariantEntities.FirstOrDefault(s => s.Id == item.IdVariant);
                    if (variantUpdate == null) continue;
                    if (item.Name != null) variantUpdate.ChangeVariantName(new Name(item.Name));
                    variantUpdate.ChangePrice(new Price(item.ExtraPrice));
                }
            }




            // remove variant 
            if (productRequest.DeleteVariant != null && productRequest.DeleteVariant.Any())
            {
                var deleteVariant = productAggregate.ProductVariantEntities.Where(s => productRequest.DeleteVariant.Contains(s.Id)).ToList();  // get list need to delete

                foreach (var item in deleteVariant)
                {
                    productAggregate.DeleteVariant(item);
                }
            }


            await _product.UpdateProductAsync(productAggregate);
        }
    }
}
