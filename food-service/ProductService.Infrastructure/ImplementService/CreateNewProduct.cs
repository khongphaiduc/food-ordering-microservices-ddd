using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Aggregate;
using food_service.ProductService.Domain.Entities;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Domain.ValueObject;
using static System.Net.Mime.MediaTypeNames;

namespace food_service.ProductService.Infrastructure.ImplementService
{
    public class CreateNewProduct : ICreateNewProduct
    {
        private readonly IProductRepository _iProductRepository;
        private readonly IR2Food _r2Food;

        public CreateNewProduct(IProductRepository productRepository, IR2Food r2Food)
        {
            _iProductRepository = productRepository;
            _r2Food = r2Food;
        }

        public async Task<bool> ExecuteAsync(CreateNewProductDTO request)
        {

            var productAggregate = ProductAggregate.CreateNewProduct(request.IdCategory, new Name(request.Name), new Price(request.Price), request.Description);

            if (request.ImageProduct != null)
            {
                foreach (var image in request.ImageProduct)
                {
                    var imageName = await _r2Food.UploadAsync(image.image);// luu vào R2 then returen name image 
                    productAggregate.AddNewImage(ProductImagesEntity.CreateNewImage(productAggregate.Id, imageName, image.IsMain));
                }
            }


            if (request.MainImage != null)
            {
                var imageName = await _r2Food.UploadAsync(request.MainImage.image);
                productAggregate.AddNewImage(ProductImagesEntity.CreateNewImage(productAggregate.Id, imageName, true));
            }


            var resultAdd = await _iProductRepository.AddProductAsync(productAggregate);

            return resultAdd;
        }


    }
}
