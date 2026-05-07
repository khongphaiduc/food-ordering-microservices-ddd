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
            
        }

    }
}
