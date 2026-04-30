using food_service.ProductService.API.gRPC;
using food_service.ProductService.API.Middlwares;
using food_service.ProductService.Application.Interface;
using food_service.ProductService.Application.Service;
using food_service.ProductService.Domain.Interface;
using food_service.ProductService.Infastructure.BackgroundServices;
using food_service.ProductService.Infastructure.ImplementService;
using food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.Consumers;
using food_service.ProductService.Infastructure.MasstransitProducerRabbitMQ.Producer;
using food_service.ProductService.Infastructure.MinIO;
using food_service.ProductService.Infastructure.Models;
using food_service.ProductService.Infastructure.Persistence;
using food_service.ProductService.Infastructure.ProducerRabbitMQ;
using food_service.ProductService.Infastructure.RedisService.RedisInterface;
using food_service.ProductService.Infastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Serilog;
using StackExchange.Redis;
using trackingtService.API.Protos;

namespace food_service.ProductService.Start
{
    public class Program
    {
        public static void Main(string[] args)
        {

            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAllServices(builder.Configuration);
            builder.Services.AddControllers();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodProductsDbContext>();
                    context.Database.Migrate();
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Running my migration is success");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "The Product Food Service has a conflict when initializing the migration");
                }
            }

            app.MapGrpcService<ProductInformationsServices>();

            app.MapGrpcService<LoadFullProduct>();

            app.UseRateLimiter();

            app.UseMiddleware<CustomGlobalException>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
