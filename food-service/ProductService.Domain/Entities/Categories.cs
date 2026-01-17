using food_service.ProductService.Domain.Aggragate;
using food_service.ProductService.Domain.ValueOject;

namespace food_service.ProductService.Domain.Entities
{
    public class Categories
    {

        public Guid Id { get; private set; }
        public Name NameCategory { get; private set; }
        public string Description { get; private set; }

        public bool IsAcctive { get; private set; }

        public DateTime CreateAt { get; private set; }

        public DateTime UpdateAt { get; private set; }

        private readonly List<ProductAggregate> _productAggregates = new();

        public IReadOnlyCollection<ProductAggregate> ProductAggregates => _productAggregates.AsReadOnly();

        internal Categories(Guid id, Name name, string description, bool isAcctive, DateTime createAt, DateTime updateAt)
        {
            Id = id;
            NameCategory = name;
            Description = description;
            IsAcctive = isAcctive;
            CreateAt = createAt;
            UpdateAt = updateAt;
        }

        private Categories()
        {
        }

        public static Categories Create(Name name, string description)
        {
            return new Categories
            {
                Id = Guid.NewGuid(),
                NameCategory = name,
                Description = description,
                IsAcctive = true,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };
        }

        public void AddProductAggregate(ProductAggregate productAggregate)
        {
            _productAggregates.Add(productAggregate);
        }


    }
}
