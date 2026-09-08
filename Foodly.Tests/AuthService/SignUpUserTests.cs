using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.ServiceImplement;
using Microsoft.Extensions.Logging;
using Moq;

namespace Foodly.Tests.AuthService;

public class SignUpUserTests
{
 


    private static SignUpUser CreateService(
        IUserRepository? userRepository = null,
        IHashPassword? hashPassword = null)
    {
        var salt = new Mock<IGenerateSalt>();
        salt.Setup(x => x.GenerateSalt()).Returns("salt");

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
