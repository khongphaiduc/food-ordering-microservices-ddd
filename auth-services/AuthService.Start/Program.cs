using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.API.Middlwares;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.BackgroundServices;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.Persistence;
using auth_services.AuthService.Infastructure.RabbitMQs.Producer;
using auth_services.AuthService.Infastructure.Reposistory;
using auth_services.AuthService.Infastructure.Security;
using auth_services.AuthService.Infastructure.ServiceImpelemt;
using auth_services.AuthService.Infastructure.Tokens;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Threading.RateLimiting;
using UserService.API.Protos;

namespace auth_services.AuthService.Start
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddPersistence(builder.Configuration);

            builder.Services.AddControllers();

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodAuthContext>();
                    context.Database.Migrate();
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Đã chạy Migration cho database thành công.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Đã xảy ra lỗi trong quá trình tự động migrate database.");
                }
            }
            app.UseRateLimiter();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionCustom>();
            app.MapControllers();
            app.Run();
        }
    }
}
