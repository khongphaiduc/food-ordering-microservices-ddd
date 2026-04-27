using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using user_service.userservice.infastructure.DBcontextService;
using user_service.UserService.API.gRPC;
using user_service.UserService.Application.Services;
using user_service.UserService.Domain.Interfaces;
using user_service.UserService.Infastructure.RabbitMQConsumers;
using user_service.UserService.Infastructure.Repository;
using user_service.UserService.Infastructure.ServiceImplement;
using UserService.API.Protos;


namespace user_service.userservice.start
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FoodUsersContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["URL_SQL_USER_SERVICE"]);
            });

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
                };
            });

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserProfile, UserProfile>();
            builder.Services.AddHostedService<UserInfoConsumer>();
            builder.Services.AddScoped<IGetInformationUser, GetInformationUser>();
            builder.Services.AddScoped<ICreateNewAddressForUser, CreateNewAddressForUser>();
            builder.Services.AddControllers();
            builder.Services.AddGrpc();// 1. Add gRPC


            //builder.Services.AddOpenTelemetry()
            //    .WithTracing(tracing =>
            //    {
            //        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("UserInfoService"))
            //               .AddAspNetCoreInstrumentation()
            //               .AddHttpClientInstrumentation()
            //               .AddEntityFrameworkCoreInstrumentation()
            //               .AddConsoleExporter()
            //               .AddOtlpExporter(opt =>
            //                 {
            //                     opt.Endpoint = new Uri("http://localhost:4318");
            //                     opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            //                 });
            //    });


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodUsersContext>();
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

            app.MapGrpcService<UserInfoSerivce>();
            app.MapGrpcService<AddressUserService>();

            app.MapGet("/", () => "gRPC Server running");

            app.UseRouting();

          
            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();

            app.Run();
        }
    }
}
