using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using System.Reflection;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
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