using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Interfaces;
using Onion.Data.Contexts;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Onion.Data.Repositories;

namespace Onion.Data
{
    public static class DependencyInjection
    {
        public static void AddDataInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<AppDbContext>(options => options
                    .UseInMemoryDatabase("ApplicationDb")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            }
            else
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b =>
                        {
                            Assembly asm = typeof(AppDbContext).Assembly;
                            b.MigrationsAssembly(asm.FullName);
                        }));
            }

            services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());
            services.TryAddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        }
    }
}