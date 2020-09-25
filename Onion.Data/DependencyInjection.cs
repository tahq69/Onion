using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Interfaces;
using Onion.Data.Contexts;
using Onion.Shared.Data;
using System.Reflection;

namespace Onion.Data
{
    public static class DependencyInjection
    {
        public static void AddDataInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b =>
                    {
                        Assembly asm = typeof(AppDbContext).Assembly;
                        b.MigrationsAssembly(asm.FullName);
                    }));

            services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());
        }
    }
}