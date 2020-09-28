using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Onion.Identity.Models;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Onion.Web
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The application entry class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The entry arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            // Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            var host = Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                    await Identity.Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
                    await Identity.Seeds.DefaultSuperAdmin.SeedAsync(userManager, roleManager);
                    await Identity.Seeds.DefaultBasicUser.SeedAsync(userManager, roleManager);
                    Log.Information("Finished Seeding Default Data");
                    Log.Information("Application Starting");
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "An error occurred seeding the DB");
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }

            host.Run();
        }

        /// <summary>
        /// Creates the host builder instance for web application.
        /// </summary>
        /// <param name="args">The entry arguments.</param>
        /// <returns>Application host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}