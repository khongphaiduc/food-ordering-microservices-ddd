using food_service.ProductService.Domain.ValueObject;

namespace food_service.ProductService.Domain.Aggregate
{
    public class CategoryAggregate
    {
        public Guid Id { get; private set; }

        public Name Name { get; private set; }

        public string Description { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreateAt { get; private set; }

        public DateTime UpdateAt { get; private set; }

        internal CategoryAggregate(Guid id, Name name, string description, bool isActive, DateTime createAt, DateTime updateAt)
        {
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
            CreateAt = createAt;
            UpdateAt = updateAt;
        }

        private CategoryAggregate()
        {
        }
        public static CategoryAggregate CreateNewCategory(Name NameCategory, string Description)
        {
            return new CategoryAggregate
            {
                Id = Guid.NewGuid(),
                Name = NameCategory,
                Description = Description,
                IsActive = true,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,

            };
        }

        public void ChangeDescription(string DescriptionNew)
        {
            Description = DescriptionNew;
            UpdateAt = DateTime.UtcNow;
        }

        public void ChangeIsActiveOrUnActive(bool status)
        {
            IsActive = status;
            UpdateAt = DateTime.UtcNow;
        }


        public void ChangeName(Name newName)
        {
            Name = newName;
        }

    }
}
