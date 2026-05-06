using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.RabbitMQs.Producer;
using auth_services.AuthService.Infastructure.ServiceImpelemt;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProto = UserService.API.Protos;
using Microsoft.Extensions.Configuration;
namespace Foodly.Tests.AuthService
{
    // Fomat : [Tên phương thức muốn test ]_[Kết quả mong đợi]_[Điều kiện thực hiện test(khi tham số là null,khi người dùng tự follow chính mình, khi database bị ngắt kết nối...)] 
    // SetUp : giả lập dữ liệu mặc định trả về khi gọi 1 method của dependency 
    // Verify() dùng để xác nhận một method của dependency có được gọi hay không.
    public class UseRegisterAccountTests
    {
        [Fact]
        public async Task UserRegisterNewAccount_ShouldRegisterSuccessfully_WhenUserFillCorrectly()
        {
            // ========== ARRANGE ==========
            var mockRepo = new Mock<IUserRepository>();
            var mockSalt = new Mock<IGenarateSalt>();
            var mockHash = new Mock<IHashPassword>();
            var mockOutbox = new Mock<IOutBoxMessage>();
            var mockLogger = new Mock<ILogger<SignUpUser>>();

            // fake DbContext (chỉ cần mock SaveChanges)
            var mockDb = new Mock<FoodAuthContext>();

            //gRPC  

            var mockUserClient = new Mock<UserServicesClient>();

            // setup dữ liệu
            mockRepo.Setup(x => x.IsExitUser(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

            mockSalt.Setup(x => x.GenarateSalt())
                    .Returns("salt123");

            mockHash.Setup(x => x.HandleHashPassword(It.IsAny<string>(), "salt123"))
                    .Returns("hashedPassword");

            // mock transaction (cái này trick nhẹ cho qua)
            //var mockTransaction = new Mock<IDbContextTransaction>();
            //mockDb.Setup(x => x.Database.BeginTransactionAsync(default))
            //      .ReturnsAsync(mockTransaction.Object);

            mockDb.Setup(x => x.SaveChangesAsync(default))
                  .ReturnsAsync(1);

            var service = new SignUpUser(
                mockLogger.Object,
                mockSalt.Object,
                mockHash.Object,
                mockRepo.Object,
                mockUserClient.Object,
                mockDb.Object,
                mockOutbox.Object
            );

            var request = new RequestCreateNewUser
            {
                Email = "test@gmail.com",
                Password = "123",
                ConfirmPassword = "123",
                UserName = "duc"
            };

            // ========== ACT ==========
            var result = await service.Execute(request);

            // ========== ASSERT ==========
            Assert.True(result);

            // verify có gọi repo add user
            mockRepo.Verify(x => x.AddNewUser(It.IsAny<UserAggregate>(), It.IsAny<CancellationToken>()), Times.Once);

            // verify có tạo outbox
            mockOutbox.Verify(x => x.CreateNewMessage(It.IsAny<OutBoxMessageInternalDTO>()), Times.Once);

            //// verify commit transaction
            //mockTransaction.Verify(x => x.CommitAsync(default), Times.Once);
        }

    }
}
