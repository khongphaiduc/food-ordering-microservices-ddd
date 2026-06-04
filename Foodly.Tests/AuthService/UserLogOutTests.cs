using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.ServiceImplement;
using Moq;

namespace Foodly.Tests.AuthService;

public class UserLogOutTests
{
    [Fact]
    public async Task Execute_RevokesRefreshTokenForUser()
    {
        var userId = Guid.NewGuid();
        var refreshTokens = new Mock<IRefreshTokenRepository>();
        refreshTokens.Setup(x => x.RevokedToken(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new UserLogOut(refreshTokens.Object);

        var result = await service.Execute(userId);

        Assert.True(result);
        refreshTokens.Verify(x => x.RevokedToken(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
