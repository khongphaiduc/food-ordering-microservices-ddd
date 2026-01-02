using auth_service.authservice.api.Middlewares;
using auth_service.authservice.application.handler;
using auth_service.authservice.application.InterfaceApplication;
using auth_service.authservice.domain.Interfaces;
using auth_service.authservice.infastructure.dbcontexts;
using auth_service.authservice.infastructure.Repository;
using auth_service.authservice.infastructure.Securities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace auth_service.authservice.start
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<FoodAuthContext>(option => option.UseSqlServer("Data Source=PHAMTRUNGDUC\\SQLEXPRESS;Initial Catalog=food_auth;Persist Security Info=True;User ID=sa;Password=123;Trust Server Certificate=True"));


            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
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


            builder.Services.AddTransient<ICreateUserHandler, CreateUserHandler>();
            builder.Services.AddTransient<IHashPassword, EncodePassword>();
            builder.Services.AddTransient<IUserRepositories, UserRepositories>();
            builder.Services.AddTransient<IUserLogin, UserSignInHandler>();
            builder.Services.AddTransient<IAuthenticationToken, AuthenticationJWT>();
            builder.Services.AddTransient<IRefreshTokensRepositories, RefreshTokensRepositories>();
            builder.Services.AddTransient<IRoleUser, RoleUser>();
            builder.Services.AddTransient<IProvideoAccessToken, ProvideoAccessToken>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();   // custom exception middleware


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
