using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.ServiceImpelemt;
using Microsoft.Extensions.Logging;
using Moq;

namespace Foodly.Tests.AuthService;

public class SignUpUserTests
{
    [Fact]
    public async Task Execute_PasswordDoesNotMatch_ReturnsFalseAndDoesNotCheckRepository()
    {
        var userRepository = new Mock<IUserRepository>();
        var service = CreateService(userRepository: userRepository.Object);

        var result = await service.Execute(new RequestCreateNewUser
        {
            UserName = "Duc",
            Email = "duc@example.com",
            Password = "Password123!",
            ConfirmPassword = "OtherPassword123!"
        });

        Assert.False(result);
        userRepository.Verify(x => x.IsExitUser(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Execute_EmailAlreadyExists_ReturnsFalseBeforeCreatingUser()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(x => x.IsExitUser("duc@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var hashPassword = new Mock<IHashPassword>();
        var service = CreateService(userRepository: userRepository.Object, hashPassword: hashPassword.Object);

        var result = await service.Execute(new RequestCreateNewUser
        {
            UserName = "Duc",
            Email = "duc@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        });

        Assert.False(result);
        hashPassword.Verify(x => x.HandleHashPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        userRepository.Verify(x => x.AddNewUser(It.IsAny<auth_services.AuthService.Domain.Aggregate.UserAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static SignUpUser CreateService(
        IUserRepository? userRepository = null,
        IHashPassword? hashPassword = null)
    {
        var salt = new Mock<IGenarateSalt>();
        salt.Setup(x => x.GenarateSalt()).Returns("salt");

        return new SignUpUser(
            Mock.Of<ILogger<SignUpUser>>(),
            salt.Object,
            hashPassword ?? Mock.Of<IHashPassword>(),
            userRepository ?? Mock.Of<IUserRepository>(),
            null!,
            null!,
            Mock.Of<IOutBoxMessage>());
    }
}
