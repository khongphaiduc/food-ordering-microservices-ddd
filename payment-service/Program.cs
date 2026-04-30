using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.API.Proto;
using payment_service.PaymentService.API.gRPC;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Infastructure.Models;
using payment_service.PaymentService.Infastructure.NotificationRealTimeSignalR;
using payment_service.PaymentService.Infastructure.Persistence;


namespace payment_service
{
    public class Program
    {
        public static void Main(string[] args)
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
                    var context = services.GetRequiredService<FoodPaymentContext>();
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

            app.UseRouting();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            

            app.MapGrpcService<CreatePaymentServers>();

            app.MapHub<NotificationPaidSusscessfully>("/notificationPayOS")
     .RequireCors("AllowFrontend");

            app.MapControllers();

            app.Run();
        }
    }
}
