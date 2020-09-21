using Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Onion.Application.Interfaces;

namespace Onion.Shared.Data
{
    public static class DependencyInjection
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MailSettings>(config.GetSection("MailSettings"));
            services.TryAddTransient<IDateTimeService, DateTimeService>();
            services.TryAddTransient<IEmailService, EmailService>();
        }
    }
}