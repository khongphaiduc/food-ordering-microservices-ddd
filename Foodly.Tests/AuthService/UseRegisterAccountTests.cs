using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Domain.Aggregate;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.RabbitMQs.Producer;
using auth_services.AuthService.Infrastructure.ServiceImplement;
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
    // Format : [Tên phuong th?c mu?n test ]_[K?t qu? mong d?i]_[Ði?u ki?n th?c hi?n test(khi tham s? là null,khi ngu?i dùng t? follow chính mình, khi database b? ng?t k?t n?i...)] 
    // SetUp : gi? l?p d? li?u m?c d?nh tr? v? khi g?i 1 method c?a dependency 
    // Verify() dùng d? xác nh?n m?t method c?a dependency có du?c g?i hay không.
    public class UseRegisterAccountTests
    {
        [Fact]
        public async Task UserRegisterNewAccount_ShouldRegisterSuccessfully_WhenUserFillCorrectly()
        {
            
        }

    }
}
