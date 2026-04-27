using Microsoft.EntityFrameworkCore;
using productService.API.Protos;
using tracking_service.Tracking.API.gRPCimplement;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Domain.Repository;
using tracking_service.Tracking.Infastructrure.ImplementServices;
using tracking_service.Tracking.Infastructrure.Models;
using tracking_service.Tracking.Infastructrure.Repo;
using tracking_service.Tracking.Infastructrure.Worker;

namespace tracking_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FoodProductsDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration["SQL_URL"]);
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis"];
                options.InstanceName = "FoodAppShared_";
            });


            builder.Services.AddGrpcClient<ProductListGrpc.ProductListGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:5002");
            });


            builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            builder.Services.AddScoped<IUserBehaviorTracking, UserBehaviorTracking>();
            builder.Services.AddScoped<IProducerTracking, ProducerTracking>();
            builder.Services.AddScoped<IServiceAI, ServiceAI>();
            builder.Services.AddHostedService<TrakingComsumer>();
            builder.Services.AddScoped<IGetBehaviourOfUser, GetBehaviourOfUser>();
            builder.Services.AddScoped<LoadFullProductService>();

            builder.Services.AddGrpc();
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
                    logger.LogInformation("Đã chạy Migration cho database thành công.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Đã xảy ra lỗi trong quá trình tự động migrate database.");
                }
            }

            app.MapGrpcService<GetResultCommendFoodService>();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
