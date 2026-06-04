using food_service.ProductService.Application.DTOs.Request;
using food_service.ProductService.Domain.Aggregate;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Domain.ValueObject;
using food_service.ProductService.Infrastructure.ImplementService;
using Moq;

namespace Foodly.Tests.FoodService;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateNewCategory_ValidRequest_CreatesActiveCategory()
    {
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(x => x.AddNewCategory(It.IsAny<CategoryAggregate>())).ReturnsAsync(true);
        var service = new CreateNewCategory(repository.Object);

        var result = await service.ExecuteAsync(new CreateNewCategoryDTO
        {
            Name = "Pizza",
            Description = "Hot food"
        });

        Assert.True(result);
        repository.Verify(x => x.AddNewCategory(It.Is<CategoryAggregate>(c =>
            c.Name.Value == "Pizza" &&
            c.Description == "Hot food" &&
            c.IsActive)), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_CategoryDoesNotExist_ReturnsFalse()
    {
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(x => x.GetCategoryById(It.IsAny<Guid>()))
            .ReturnsAsync((CategoryAggregate)null!);
        var service = new UpdateCategory(repository.Object);

        var result = await service.ExecuteAsync(new UpdateCategoryDTO
        {
            Id = Guid.NewGuid(),
            Name = "Burger"
        });

        Assert.False(result);
        repository.Verify(x => x.UpdateCategory(It.IsAny<CategoryAggregate>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCategory_CategoryExists_UpdatesProvidedFieldsOnly()
    {
        var category = CategoryAggregate.CreateNewCategory(new Name("Pizza"), "Old description");
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(x => x.GetCategoryById(category.Id)).ReturnsAsync(category);
        repository.Setup(x => x.UpdateCategory(It.IsAny<CategoryAggregate>())).ReturnsAsync(true);
        var service = new UpdateCategory(repository.Object);

        var result = await service.ExecuteAsync(new UpdateCategoryDTO
        {
            Id = category.Id,
            Name = "Burger",
            Description = "New description",
            IsActive = false
        });

        Assert.True(result);
        repository.Verify(x => x.UpdateCategory(It.Is<CategoryAggregate>(c =>
            c.Name.Value == "Burger" &&
            c.Description == "New description" &&
            !c.IsActive)), Times.Once);
    }
}
