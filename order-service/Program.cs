using CartService.API.Protos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using order_service.OrderService.API.gRPC;
using order_service.OrderService.Appilcation.Interface;
using order_service.OrderService.Appilcation.Services;
using order_service.OrderService.Domain.Interface;
using order_service.OrderService.Infastructure.Models;
using order_service.OrderService.Infastructure.OrderRealTime;
using order_service.OrderService.Infastructure.Persistence;
using order_service.OrderService.Infastructure.Repository;
using order_service.OrderService.Infastructure.ServicesImplements;
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
                    logger.LogInformation("Đã chạy Migration cho database thành công.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Đã xảy ra lỗi trong quá trình tự động migrate database.");
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