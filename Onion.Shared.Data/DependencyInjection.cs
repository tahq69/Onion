using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Onion.Application.Interfaces;

namespace Onion.Shared.Data
{
    public static class DependencyInjection
    {
        public static void AddSharedDataInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}