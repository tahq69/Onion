using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Interfaces;
using Onion.Data.Contexts;
using Onion.Identity.Contexts;
using Onion.Identity.Models;

namespace Onion.Web.IntegrationTests
{
    public class InMemoryDatabaseAppFactory : WebApplicationFactory<Startup>
    {
        public InMemoryDatabaseAppFactory()
        {
            DatabaseName = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the name of the database for current tests.
        /// </summary>
        public string DatabaseName { get; }

        public IAppDbContext AppDbContext =>
            Services.CreateScope().ServiceProvider.GetRequiredService<IAppDbContext>();

        public IdentityDbContext IdentityDbContext =>
            Services.CreateScope().ServiceProvider.GetRequiredService<IdentityDbContext>();

        protected override IWebHostBuilder CreateWebHostBuilder() =>
            WebHost.CreateDefaultBuilder().UseStartup<Startup>();

        private void Remove<T>(IServiceCollection services)
        {
            ServiceDescriptor descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(T));

            if (descriptor != null)
                services.Remove(descriptor);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //// string mockServer = $"http://localhost:{ServerMock.Port}/";
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration(config =>
            {
                config.Add(new JsonConfigurationSource { Path = "appsettings.Testing.json" });
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:HealthCheckDatabase", $"Data Source={Guid.NewGuid()}" },
                    //// { "ServiceConfiguration:ResultsWebServiceUrl", mockServer },
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddEntityFrameworkInMemoryDatabase();
                ServiceProvider sp = services.BuildServiceProvider();

                using IServiceScope scope = sp.CreateScope();

                var serviceProvider = scope.ServiceProvider;
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                Task.Run(async () =>
                {
                    await Identity.Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
                    await Identity.Seeds.UnitTestUsers.SeedAsync(userManager, roleManager);
                }).Wait();
            });

            base.ConfigureWebHost(builder);
        }
    }
}