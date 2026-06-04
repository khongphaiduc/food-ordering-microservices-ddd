using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using user_service.userservice.infastructure.DBcontextService;
using user_service.userservice.infastructure.Models;
using user_service.UserService.Application.DTOS;
using user_service.UserService.Domain.Aggregates;
using user_service.UserService.Domain.Interfaces;
using user_service.UserService.Infastructure.ServiceImplement;

namespace Foodly.Tests.UserService;

public class UserProfileTests
{
    [Fact]
    public async Task UserProfilHandle_NewEmailAndUserDoesNotExist_AddsUser()
    {
        await using var db = CreateContext();
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.IsEmailExistsAsync("duc@example.com")).ReturnsAsync(false);
        repository.Setup(x => x.AddNewUserAsync(It.IsAny<UserAggregate>())).ReturnsAsync(true);

        var service = new UserProfile(db, repository.Object, Mock.Of<ILogger<UserProfile>>());

        var result = await service.UserProfilHandle(new RequestUserProfile
        {
            Id = Guid.NewGuid(),
            FullName = "Pham Trung Duc",
            Email = "duc@example.com",
            PhoneNumber = "0987654321"
        });

        Assert.True(result);
        repository.Verify(x => x.AddNewUserAsync(It.Is<UserAggregate>(u => u.Email.Value == "duc@example.com")), Times.Once);
    }

    [Fact]
    public async Task UserProfilHandle_EmailAlreadyExists_ReturnsFalse()
    {
        await using var db = CreateContext();
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.IsEmailExistsAsync("duc@example.com")).ReturnsAsync(true);

        var service = new UserProfile(db, repository.Object, Mock.Of<ILogger<UserProfile>>());

        var result = await service.UserProfilHandle(new RequestUserProfile
        {
            Id = Guid.NewGuid(),
            FullName = "Pham Trung Duc",
            Email = "duc@example.com",
            PhoneNumber = "0987654321"
        });

        Assert.False(result);
        repository.Verify(x => x.AddNewUserAsync(It.IsAny<UserAggregate>()), Times.Never);
    }

    [Fact]
    public async Task UserProfilHandle_UserAlreadyExists_ReturnsFalse()
    {
        await using var db = CreateContext();
        var userId = Guid.NewGuid();
        db.Users.Add(new User
        {
            Id = userId,
            FullName = "Existing User",
            Email = "existing@example.com",
            PhoneNumber = "0900000000",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.IsEmailExistsAsync("duc@example.com")).ReturnsAsync(false);

        var service = new UserProfile(db, repository.Object, Mock.Of<ILogger<UserProfile>>());

        var result = await service.UserProfilHandle(new RequestUserProfile
        {
            Id = userId,
            FullName = "Pham Trung Duc",
            Email = "duc@example.com",
            PhoneNumber = "0987654321"
        });

        Assert.False(result);
        repository.Verify(x => x.AddNewUserAsync(It.IsAny<UserAggregate>()), Times.Never);
    }

    private static FoodUsersContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FoodUsersContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FoodUsersContext(options);
    }
}
