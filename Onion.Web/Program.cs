using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Onion.Identity.Models;
using Onion.Identity.Seeds;
using Serilog;
using Serilog.Core;

namespace Onion.Web
{
    /// <summary>
    /// The application entry class.
    /// </summary>
    public static class Program
    {
        private static ILogger Log => Serilog.Log.ForContext(typeof(Program));

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The entry arguments.</param>
        public static void Main(string[] args)
        {
            Serilog.Log.Logger = CreateLogger();
            Log.Information("Starting application");

            CreateHostBuilder(args).Build()
                .SeedDatabase().GetAwaiter().GetResult()
                .Run();

            Log.Information("Application exit");
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());


        private static Logger CreateLogger()
        {
            var configuration = BuildConfiguration();
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static async Task<IHost> SeedDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            try
            {
                await SeedDatabase(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "An error occurred seeding the DB");
            }

            return host;
        }

        private static async Task SeedDatabase(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            await DefaultRoles.SeedAsync(userManager, roleManager);
            await DefaultSuperAdmin.SeedAsync(userManager, roleManager);
            await DefaultBasicUser.SeedAsync(userManager, roleManager);
            Log.Information("Finished seeding default data");
        }
    }
}