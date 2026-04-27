using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.API.Middlwares;
using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.BackgroundServices;
using auth_services.AuthService.Infastructure.DbContextAuth;
using auth_services.AuthService.Infastructure.RabbitMQs.Producer;
using auth_services.AuthService.Infastructure.Reposistory;
using auth_services.AuthService.Infastructure.Security;
using auth_services.AuthService.Infastructure.ServiceImpelemt;
using auth_services.AuthService.Infastructure.Tokens;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Threading.RateLimiting;
using UserService.API.Protos;

namespace auth_services.AuthService.Start
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FoodAuthContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["URLSQLFOOD_ORDRING_AUTH_SERVICE"]);
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
            }).AddJwtBearer("RefreshToken", option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:RefreshToken"]!))
                };
            });


            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("token", context =>
                    RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: context.User.Identity?.Name ?? "anonymous",
                        factory: _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 10,
                            TokensPerPeriod = 5,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }));
            });


            builder.Services.AddGrpcClient<UserInfoGrpc.UserInfoGrpcClient>(s =>
            {
                s.Address = new Uri("https://localhost:5001");
            });


            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis:Host"];
                options.InstanceName = "Foodly";
            });


            builder.Services.AddHttpContextAccessor();



            builder.Services.AddValidatorsFromAssemblyContaining<Program>();  // scan all project 
            builder.Services.AddFluentValidationAutoValidation();            // auto


            builder.Services.AddScoped<IGenarateSalt, GenarateSalt>();
            builder.Services.AddScoped<IHashPassword, HashPassword>();
            builder.Services.AddScoped<ISignUpUser, SignUpUser>();
            builder.Services.AddScoped<ICheckLogin, CheckLogin>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProvideAccessToken, ProvideAccessToken>();
            builder.Services.AddScoped<IUserLogOut, UserLogOut>();
            builder.Services.AddSingleton<UserServicesClient>();
            // token
            builder.Services.AddScoped<IGanarateTokenJWT, GanarateAccessTokenJWT>();
            builder.Services.AddScoped<IGanarateTokenJWT, GanarateRefresheTokenJWT>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddSingleton<RabbitMQProducer>();

            builder.Services.AddScoped<IOutBoxMessage, OutBoxMessage>();



            builder.Services.AddHostedService<OutBoxWorker>();
            builder.Services.AddScoped<IAddAccountStaffs, AddAccountStaffs>();
            builder.Services.AddScoped<IAddSafttRepository, AddSafttRepository>();
            builder.Services.AddScoped<IGetListStaff, GetListStaff>();
            builder.Services.AddControllers();

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FoodAuthContext>();
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

            app.UseRateLimiter();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<GlobalExceptionCustom>();
            app.MapControllers();

            app.Run();
        }
    }
}
