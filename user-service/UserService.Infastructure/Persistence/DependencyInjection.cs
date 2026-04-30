using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using user_service.UserService.Application.Services;
using user_service.userservice.infastructure.DBcontextService;
using user_service.UserService.Infastructure.RabbitMQConsumers;
using user_service.UserService.Infastructure.ServiceImplement;
using user_service.UserService.Infastructure.Repository;
using user_service.UserService.Domain.Interfaces;

namespace user_service.UserService.Infastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services, IConfiguration config)
        {
           
            services.AddDbContext<FoodUsersContext>(options =>
            {
                options.UseSqlServer(config["URL_SQL_USER_SERVICE"]);
            });

           
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidAudience = config["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(config["JWT:Key"]!)
                    )
                };
            });

          
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserProfile, UserProfile>();
            services.AddScoped<IGetInformationUser, GetInformationUser>();
            services.AddScoped<ICreateNewAddressForUser, CreateNewAddressForUser>();

           
            services.AddHostedService<UserInfoConsumer>();

           
            services.AddControllers();
            services.AddGrpc();

         
            //services.AddOpenTelemetry()
            //    .WithTracing(tracing =>
            //    {
            //        tracing.SetResourceBuilder(
            //                    ResourceBuilder.CreateDefault().AddService("UserInfoService"))
            //               .AddAspNetCoreInstrumentation()
            //               .AddHttpClientInstrumentation()
            //               .AddEntityFrameworkCoreInstrumentation()
            //               .AddConsoleExporter()
            //               .AddOtlpExporter(opt =>
            //               {
            //                   opt.Endpoint = new Uri("http://localhost:4318");
            //                   opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            //               });
            //    });

            return services;
        }
    }
}
