using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build(); 

            using var scope = host.Services.CreateScope();  // Host created services

            var services = scope.ServiceProvider;   

            try
            {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync(); // Makes any necessary migrations and Creates DB if it does not already exist
                await Seed.SeedData(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();   // Log error if something goes wrong
                logger.LogError(ex, "An error occured during migration!");
            }

            await host.RunAsync(); // Run the application
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
