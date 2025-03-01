using ConsoleLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PRSApi.Models;

namespace PRSApi {

    public class Program {

        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddControllers().AddJsonOptions(opt => {
                opt.JsonSerializerOptions.ReferenceHandler=
                  System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; opt.JsonSerializerOptions.WriteIndented=true;
            });
            // Configure the HTTP request pipeline.
            builder.Services.AddDbContext<PRSContext>(
               // lambda
               options => options.UseSqlServer(builder.Configuration.GetConnectionString("PRSConnectionString"))
            );
            var app = builder.Build();
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
