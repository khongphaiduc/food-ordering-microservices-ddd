namespace food_service.ProductService.Domain.Entities
{
    public class ProductImagesEntity
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        internal ProductImagesEntity(Guid id, Guid productId, string imageUrl, bool isMain)
        {
            Id = id;
            ProductId = productId;
            ImageUrl = imageUrl;
            IsMain = isMain;
        }
        private ProductImagesEntity()
        {

        }
        public static ProductImagesEntity CreateNewImage(Guid IdProduct, string ImageUrl, bool IsMain)
        {
            return new ProductImagesEntity
            {
                Id = Guid.NewGuid(),
                ProductId = IdProduct,
                ImageUrl = ImageUrl,
                IsMain = IsMain
            };
        }

        public void ChangeUrl(string newUrl)
        {
            ImageUrl = newUrl;
        }

        public void SetAsMainImage()
        {
            IsMain = true;
        }

        public void UnsetAsMainImage()
        {
            IsMain = false;
        }
    }
}
