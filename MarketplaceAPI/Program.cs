using MarketplaceData.Services.Logger;
using Microsoft.EntityFrameworkCore;
using Scalar;
using Scalar.AspNetCore;
using Serilog;
using VetAPI.Services;
using VetClassLibrary.Interfaces;
using VetClassLibrary.Services;

namespace MarketplaceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                            .WriteTo.File("logs/audit.txt",
                                rollingInterval: RollingInterval.Day,
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();

            builder.Host.UseSerilog();


            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddScoped<IApiUserAuthService, ApiUserAuthService>();
            builder.Services.AddScoped<IUserService, UserService>();


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=Marketplace.db"));


            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();


            app.UseMiddleware<AuditLoggingMiddleware>();
            app.UseMiddleware<GlobalExeptionHandler>();

            app.UseAuthorization();


            app.MapControllers();

            try
            {
                Log.Information("Starting web host");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
