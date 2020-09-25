using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Onion.Application.Interfaces;
using Onion.Domain.Settings;
using Onion.Shared.Data;

namespace Onion.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MailSettings>(config.GetSection("MailSettings"));
            services.TryAddTransient<IDateTimeService, DateTimeService>();
        }
    }
}