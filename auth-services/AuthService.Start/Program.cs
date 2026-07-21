using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.API.Middlewares;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.BackgroundServices;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.Persistence;
using auth_services.AuthService.Infrastructure.RabbitMQs.Producer;
using auth_services.AuthService.Infrastructure.Repository;
using auth_services.AuthService.Infrastructure.Security;
using auth_services.AuthService.Infrastructure.ServiceImplement;
using auth_services.AuthService.Infrastructure.Tokens;
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
                    logger.LogInformation("Database migration completed successfully.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while applying database migrations.");
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
