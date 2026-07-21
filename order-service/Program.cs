using CartService.API.Protos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using order_service.OrderService.API.gRPC;
using order_service.OrderService.Application.Interface;
using order_service.OrderService.Application.Services;
using order_service.OrderService.Domain.Interface;
using order_service.OrderService.Infrastructure.Models;
using order_service.OrderService.Infrastructure.OrderRealTime;
using order_service.OrderService.Infrastructure.Persistence;
using order_service.OrderService.Infrastructure.Repository;
using order_service.OrderService.Infrastructure.ServicesImplements;
using PaymentService.API.Proto;
using UserService.API.Protos;

namespace order_service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAllServices(builder.Configuration);

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodOrderContext>();
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


            app.UseHttpsRedirection();


            app.UseCors("AllowFrontend");


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapHub<NotificationOrderHUB>("/ordersHub");
            app.MapGrpcService<UpdateStatusOrderService>();
            app.MapControllers();

            app.Run();
        }
    }
}