using food_service.ProductService.Domain.Entities;
using food_service.ProductService.Domain.ValueObject;

namespace food_service.ProductService.Domain.Aggregate
{
    public class ProductAggregate
    {
        public Guid Id { get; private set; }

        public Guid CategoryId { get; private set; }

        public Name NameProduct { get; private set; }

        public Price PriceProduct { get; private set; }

        public string Description { get; private set; }


        public bool IsAvailable { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }


        private List<ProductImagesEntity> productImagesEntities = new List<ProductImagesEntity>();
        public IReadOnlyCollection<ProductImagesEntity> ProductImagesEntities => productImagesEntities.AsReadOnly();

        private List<ProductVariantEntity> productVariantEntities = new List<ProductVariantEntity>();
        public IReadOnlyCollection<ProductVariantEntity> ProductVariantEntities => productVariantEntities.AsReadOnly();
        private ProductAggregate()
        {
        }

        internal ProductAggregate(Guid id, Guid productID, Name nameProduct, Price priceProduct, string description, bool isAvailable, bool isDeleted, DateTime createdAt, DateTime updatedAt, List<ProductImagesEntity> productImagesEntities, List<ProductVariantEntity> productVariantEntities)
        {
            Id = productID;
            CategoryId = id;
            NameProduct = nameProduct;
            PriceProduct = priceProduct;
            Description = description;
            IsAvailable = isAvailable;
            IsDeleted = isDeleted;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            this.productImagesEntities = productImagesEntities;
            this.productVariantEntities = productVariantEntities;
        }


        public static ProductAggregate CreateNewProduct(Guid idCategory, Name name, Price price, string description)
        {
            return new ProductAggregate
            {
                Id = Guid.NewGuid(),
                CategoryId = idCategory,
                NameProduct = name,
                PriceProduct = price,
                Description = description,
                IsAvailable = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void AddNewImage(ProductImagesEntity image)
        {
            productImagesEntities.Add(image);
        }

        public void AddNewVariant(ProductVariantEntity variant)
        {
            productVariantEntities.Add(variant);
        }

        public void ChangePrice(Price price)
        {
            PriceProduct = price;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeName(Name name)
        {
            NameProduct = name;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeDescription(string description)
        {
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }


        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            UpdatedAt = DateTime.UtcNow;
        }


        public void IsDeleting(bool IsChange)
        {
            IsDeleted = IsChange;

        }


        public void DeleteImage(ProductImagesEntity image)
        {
            productImagesEntities.Remove(image);
        }

        public void DeleteVariant(ProductVariantEntity variant)
        {
            productVariantEntities.Remove(variant);
        }


    }
}
