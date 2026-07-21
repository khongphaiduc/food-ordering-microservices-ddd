using auth_services.AuthService.API.gRPCs;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Application.Service;
using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infrastructure.BackgroundServices;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.RabbitMQs.Producer;
using auth_services.AuthService.Infrastructure.Repository;
using auth_services.AuthService.Infrastructure.Security;
using auth_services.AuthService.Infrastructure.ServiceImplement;
using auth_services.AuthService.Infrastructure.Tokens;
using auth_services.AuthService.Start;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Threading.RateLimiting;
using UserService.API.Protos;

namespace auth_services.AuthService.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<FoodAuthContext>(options =>
             {
                 options.UseSqlServer(configuration["URLSQLFOOD_ORDRING_AUTH_SERVICE"]);
             });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("AccessToken", option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key:AccessToken"]!))
                };
            }).AddJwtBearer("RefreshToken", option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key:RefreshToken"]!))
                };
            });



            services.AddRateLimiter(options =>
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


            services.AddGrpcClient<UserInfoGrpc.UserInfoGrpcClient>(s =>
            {
                s.Address = new Uri(configuration["gRPC_UserService_URL"]!);
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:Host"];
                options.InstanceName = "Foodly";
            });

            services.AddHttpContextAccessor();

            services.AddValidatorsFromAssemblyContaining<Program>();  // scan all project 
            services.AddFluentValidationAutoValidation();            // auto

            services.AddScoped<IGenerateSalt, GenerateSalt>();
            services.AddScoped<IHashPassword, HashPassword>();
            services.AddScoped<ISignUpUser, SignUpUser>();
            services.AddScoped<ICheckLogin, CheckLogin>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProvideAccessToken, ProvideAccessToken>();
            services.AddScoped<IUserLogOut, UserLogOut>();
            services.AddSingleton<UserServicesClient>();

          
            services.AddScoped<IGenerateTokenJWT, GenerateAccessTokenJWT>();
            services.AddScoped<IGenerateTokenJWT, GenerateRefreshTokenJWT>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<RabbitMQProducer>();

            services.AddScoped<IOutBoxMessage, OutBoxMessage>();

            services.AddHostedService<OutBoxWorker>();
            services.AddScoped<IAddAccountStaffs, AddAccountStaffs>();
            services.AddScoped<IAddStaffRepository, AddStaffRepository>();
            services.AddScoped<IGetListStaff, GetListStaff>();

            return services;
        }
    }
}
