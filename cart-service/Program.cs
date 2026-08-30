using cart_service.CartService.API.gRPC;
using cart_service.CartService.Infrastructure.Consumers;
using cart_service.CartService.Infrastructure.Models;
using cart_service.CartService.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace cart_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddPersistence(builder.Configuration);


            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<FoodProductsDbContext>(s =>
                {
                    s.UsePostgres();
                    s.UseBusOutbox(); 
                });

                x.AddConsumer<CheckOutCart>();
                x.AddConsumer<RestoreStatusCart>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration["RabbitMQ_HostName"],"/", h =>
                    {
                        h.Username(builder.Configuration["RabbitMQ_UserName"]!);
                        h.Password(builder.Configuration["RabbitMQ_Password"]!);
                    });

                    cfg.ReceiveEndpoint("CheckOutCartQueue", e =>
                    {
                        e.PrefetchCount = 10;
                        e.ConcurrentMessageLimit = 5;
                        e.UseEntityFrameworkOutbox<FoodProductsDbContext>(context);
                        e.ConfigureConsumer<CheckOutCart>(context);
                    });


                    cfg.ReceiveEndpoint("RestoreStatusCartQueue", e =>
                    {
                        e.PrefetchCount = 10;
                        e.ConcurrentMessageLimit = 5;
                        e.UseEntityFrameworkOutbox<FoodProductsDbContext>(context);
                        e.ConfigureConsumer<RestoreStatusCart>(context);
                    });
                });
            });

            var app = builder.Build();



            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodProductsDbContext>();
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
            app.MapGrpcService<CartInforService>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
