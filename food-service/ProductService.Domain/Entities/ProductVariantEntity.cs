using food_service.ProductService.Domain.ValueOject;

namespace food_service.ProductService.Domain.Entities
{
    public class ProductVariantEntity
    {

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Name VariantName { get; set; }

        public Price ExtraPrice { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }
        private ProductVariantEntity()
        {
        }

        internal ProductVariantEntity(Guid id, Guid productId, Name variantName, Price extraPrice, bool isActive, DateTime createAt, DateTime updateAt)
        {
            Id = id;
            ProductId = productId;
            VariantName = variantName;
            ExtraPrice = extraPrice;
            IsActive = isActive;
            CreateAt = createAt;
            UpdateAt = updateAt;
        }

        public static ProductVariantEntity CreateNewVariant(Guid productId, Name variantName, Price extraPrice, bool isActive)
        {
            return new ProductVariantEntity
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                VariantName = variantName,
                ExtraPrice = extraPrice,
                IsActive = isActive,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };
        }


        public void ChangePrice(Price newPrice)
        {
            ExtraPrice = newPrice;
            UpdateAt = DateTime.UtcNow;
        }


        public void ChangeVariantName(Name newName)
        {
            VariantName = newName;
            UpdateAt = DateTime.UtcNow;
        }


        public void SetActiveStatus(bool isActive)
        {
            IsActive = isActive;
            UpdateAt = DateTime.UtcNow;
        }

    }
}
