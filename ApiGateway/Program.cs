using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);


            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReact",
                    policy =>
                        policy
                            .WithOrigins(builder.Configuration["URLACCEPTANCE_FE"]!)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                );
            });

            builder.Services.AddOcelot();


            var app = builder.Build();

            app.UseCors("AllowReact");

            await app.UseOcelot();

            app.Run();
        }
    }
}
