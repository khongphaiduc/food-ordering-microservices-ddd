using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.ServiceImplement;
using auth_services.AuthService.Infrastructure.Tokens;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit; 

namespace Foodly.Tests.AuthService
{
    public class UserLoginAccount
    {
        [Fact]
        public async Task UserLogInAccount_UserLoginSuccessfully_UserFillCorrectInfor()
        {
            // arrange
            var mockHash = new Mock<IHashPassword>();
            var mockRefreshToken = new Mock<IRefreshTokenRepository>();
            var mockUserRepo = new Mock<IUserRepository>();
            var mockLogger = new Mock<ILogger<CheckLogin>>();
            var mockDistributedCache = new Mock<IDistributedCache>();


            var fakeUserId = Guid.NewGuid();
            var testEmail = "ptrungduc1011@gmail.com";
            var testPassword = "my_correct_password";
            var dbSalt = "salt123";
            var dbHashedPassword = "hashedPassword_from_db";


            mockUserRepo.Setup(x => x.GetUserByEmail(testEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserInformation
                {
                    Id = fakeUserId,
                    Email = testEmail,
                    PasswordHash = dbHashedPassword,
                    paswordSalt = dbSalt,
                    Roles = new List<string> { "Customer" },
                    Username = "2hondaicodon"
                });


            mockHash.Setup(x => x.HandleHashPassword(testPassword, dbSalt))
                .Returns(dbHashedPassword);


            mockDistributedCache.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            mockRefreshToken.Setup(x => x.AddNewRefreshToken(fakeUserId, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);


            var mockAccessTokenGen = new Mock<GenerateAccessTokenJWT>();
            mockAccessTokenGen.Setup(x => x.HandleGenerateJWT(fakeUserId, testEmail, "Customer"))
                .Returns(new TokenResponse
                {
                    CreateAt = DateTime.Now,
                    ExpireAt = DateTime.Now.AddMinutes(30),
                    TokenType = "AccessToken",
                    TokenValue = "fake_access_token",
                });

            var mockRefreshTokenGen = new Mock<GenerateRefreshTokenJWT>();
            mockRefreshTokenGen.Setup(x => x.HandleGenerateJWT(fakeUserId, testEmail, "Customer"))
                .Returns(new TokenResponse
                {
                    CreateAt = DateTime.Now,
                    ExpireAt = DateTime.Now.AddDays(7),
                    TokenType = "RefreshToken",
                    TokenValue = "fake_refresh_token",
                });


            var tokenGenerators = new List<IGenerateTokenJWT>
            {
                mockAccessTokenGen.Object,
                mockRefreshTokenGen.Object
            };

            var checkLoginService = new CheckLogin(
                mockUserRepo.Object,
                mockDistributedCache.Object,
                mockHash.Object,
                tokenGenerators,
                mockRefreshToken.Object,
                mockLogger.Object
            );

            var requestUser = new RequestUserLogin
            {
                Email = testEmail,
                Password = testPassword
            };

            // act 
            var response = await checkLoginService.IsUserLoginAsync(requestUser);

            // assert 
            Assert.NotNull(response);
            Assert.True(response.IsLoginSuccessful);
            Assert.Equal("Login successful", response.Message);
            Assert.Equal(fakeUserId, response.Id);
            Assert.Equal(testEmail, response.Email);
            Assert.Equal("fake_access_token", response.AccessToken.TokenValue);
            Assert.Equal("fake_refresh_token", response.RefreshToken.TokenValue);
            Assert.NotEqual(Guid.Empty, response.IdSession); // Ð?m b?o SessionID dã du?c t?o
        }

        [Fact]
        public async Task UserLogInAccount_UserNotFound_ReturnsFail()
        {
            // 1. ARRANGE
            var mockUserRepo = new Mock<IUserRepository>();
            var mockHash = new Mock<IHashPassword>();
            // Các mock khác không du?c g?i t?i trong case này nên không c?n setup ru?m rà
            var mockDistributedCache = new Mock<IDistributedCache>();
            var mockRefreshToken = new Mock<IRefreshTokenRepository>();
            var mockLogger = new Mock<ILogger<CheckLogin>>();

            // Gi? l?p: Repository tìm không th?y user (tr? v? null)
            mockUserRepo.Setup(x => x.GetUserByEmail(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserInformation)null);

            var checkLoginService = new CheckLogin(
                mockUserRepo.Object, mockDistributedCache.Object, mockHash.Object,
                new List<IGenerateTokenJWT>(), mockRefreshToken.Object, mockLogger.Object
            );

            var requestUser = new RequestUserLogin
            {
                Email = "taikhoan_khong_tontai@gmail.com",
                Password = "SomePassword123"
            };

            // 2. ACT
            var response = await checkLoginService.IsUserLoginAsync(requestUser);

            // 3. ASSERT
            Assert.NotNull(response);
            Assert.False(response.IsLoginSuccessful);
            Assert.Equal("User not found", response.Message);
        }


        [Fact]
        public async Task UserLogInAccount_WrongPassword_ReturnsFail()
        {
            // 1. ARRANGE
            var mockUserRepo = new Mock<IUserRepository>();
            var mockHash = new Mock<IHashPassword>();
            var mockDistributedCache = new Mock<IDistributedCache>();
            var mockRefreshToken = new Mock<IRefreshTokenRepository>();
            var mockLogger = new Mock<ILogger<CheckLogin>>();

            var testEmail = "ptrungduc1011@gmail.com";
            var dbSalt = "salt123";
            var dbHashedPassword = "correct_hashed_password"; // M?t kh?u dúng trong DB

            // Gi? l?p: User có t?n t?i trong DB
            mockUserRepo.Setup(x => x.GetUserByEmail(testEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserInformation
                {
                    Email = testEmail,
                    PasswordHash = dbHashedPassword,
                    paswordSalt = dbSalt
                });

            // Gi? l?p: Khi hash m?t kh?u sai mà user g?i lên, nó ra m?t chu?i b?y b? nào dó
            mockHash.Setup(x => x.HandleHashPassword("wrong_password", dbSalt))
                .Returns("wrong_hashed_password");

            var checkLoginService = new CheckLogin(
                mockUserRepo.Object, mockDistributedCache.Object, mockHash.Object,
                new List<IGenerateTokenJWT>(), mockRefreshToken.Object, mockLogger.Object
            );

            var requestUser = new RequestUserLogin
            {
                Email = testEmail,
                Password = "wrong_password" // User nh?p sai pass
            };

            // 2. ACT
            var response = await checkLoginService.IsUserLoginAsync(requestUser);

            // 3. ASSERT
            Assert.NotNull(response);
            Assert.False(response.IsLoginSuccessful);
            Assert.Equal("Password is incorrect", response.Message);
        }

        [Fact]
        public async Task UserLogInAccount_RequestCancelled_ThrowsTaskCanceledException()
        {
            // 1. ARRANGE
            var mockUserRepo = new Mock<IUserRepository>();
            var mockHash = new Mock<IHashPassword>();
            var mockDistributedCache = new Mock<IDistributedCache>();
            var mockRefreshToken = new Mock<IRefreshTokenRepository>();
            var mockLogger = new Mock<ILogger<CheckLogin>>();

            // T?o m?t CancellationToken dã b? hu? (Canceled)
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var cancelledToken = cancellationTokenSource.Token;

            // Gi? l?p: User Repo nh?n du?c token hu? và ném l?i TaskCanceledException
            mockUserRepo.Setup(x => x.GetUserByEmail(It.IsAny<string>(), cancelledToken))
                .ThrowsAsync(new TaskCanceledException());

            var checkLoginService = new CheckLogin(
                mockUserRepo.Object, mockDistributedCache.Object, mockHash.Object,
                new List<IGenerateTokenJWT>(), mockRefreshToken.Object, mockLogger.Object
            );

            var requestUser = new RequestUserLogin { Email = "test@gmail.com", Password = "123" };

            // 2. ACT & ASSERT
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                checkLoginService.IsUserLoginAsync(requestUser, cancelledToken));
        }


    }
}