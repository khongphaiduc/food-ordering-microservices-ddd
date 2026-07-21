using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.API.Proto;
using payment_service.PaymentService.API.gRPC;
using payment_service.PaymentService.Application.Services;
using payment_service.PaymentService.Infrastructure.Models;
using payment_service.PaymentService.Infrastructure.NotificationRealTimeSignalR;
using payment_service.PaymentService.Infrastructure.Persistence;


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
                    logger.LogInformation("Database migration completed successfully.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while applying database migrations.");
                }
            }


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            

            app.MapGrpcService<CreatePaymentServers>();

            app.MapHub<NotificationPaidSuccessfully>("/notificationPayOS")
     .RequireCors("AllowFrontend");

            app.MapControllers();

            app.Run();
        }
    }
}
