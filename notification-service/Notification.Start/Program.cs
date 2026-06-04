using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using notification_service.Models;
using notification_service.Notification.Application.Services;
using notification_service.Notification.Domain.Interface;
using notification_service.Notification.Infrastructure.Repositories;
using notification_service.Notification.Infrastructure.Worker.EmailWorker;
using notification_service.Notifications.Services;

namespace notification_service.Notification.Start
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddDbContext<FoodNotificationDbContext>(options =>
            {

                options.UseNpgsql(builder.Configuration["URLSQL_NOTIFICATIONS"]);

            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("AccessToken", option =>
             {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:AccessToken"]!))
            };
             });


            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

            builder.Services.AddSingleton<INotifications, Emails>();

            builder.Services.AddHostedService<EmailConsumer>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodNotificationDbContext>();
                    context.Database.Migrate();
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Ðã ch?y Migration cho database thành công.");
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ðã x?y ra l?i trong quá trình t? d?ng migrate database.");
                }
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
