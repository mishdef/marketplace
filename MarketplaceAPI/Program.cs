using MarketplaceData.Services.Logger;
using Microsoft.EntityFrameworkCore;
using Scalar;
using Scalar.AspNetCore;
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
                        
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();


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

            app.UseMiddleware<GlobalExeptionHandler>();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
