using Microsoft.EntityFrameworkCore;
using user_service.userservice.infastructure.DBcontextService;
using user_service.userservice.infastructure.Models;
using user_service.UserService.API.Middlewares;
using user_service.UserService.Infrastructure.ServiceImplement;

namespace Foodly.Tests.UserService;

public class GetInformationUserTests
{
    [Fact]
    public async Task Execute_UserExists_ReturnsUserInformationWithAddresses()
    {
        await using var db = CreateContext();
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        db.Users.Add(new User
        {
            Id = userId,
            FullName = "Pham Trung Duc",
            Email = "duc@example.com",
            PhoneNumber = "0987654321",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserAddresses =
            {
                new UserAddress
                {
                    Id = addressId,
                    UserId = userId,
                    AddressLine1 = "123 Main",
                    AddressLine2 = "Floor 2",
                    City = "Ho Chi Minh",
                    District = "District 1",
                    PostalCode = "700000",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            }
        });
        await db.SaveChangesAsync();

        var service = new GetInformationUser(db);

        var result = await service.Execute(userId);

        Assert.Equal(userId, result.Iduser);
        Assert.Equal("Pham Trung Duc", result.Name);
        Assert.Single(result.addressUsers);
        Assert.Equal(addressId, result.addressUsers[0].IdAddressItem);
        Assert.Equal("District 1", result.addressUsers[0].Region);
    }

    [Fact]
    public async Task Execute_UserDoesNotExist_ThrowsNotFoundUserException()
    {
        await using var db = CreateContext();
        var service = new GetInformationUser(db);

        await Assert.ThrowsAsync<NotFoundUserException>(() => service.Execute(Guid.NewGuid()));
    }

    private static FoodUsersContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FoodUsersContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FoodUsersContext(options);
    }
}
