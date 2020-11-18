using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Onion.Application.Interfaces;
using Onion.Domain.Settings;
using Onion.Infrastructure.Services;
using Onion.Logging;

namespace Onion.Infrastructure
{
    /// <summary>
    /// Infrastructure DI helper.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds Infrastructure DI.
        /// </summary>
        /// <param name="services">DI service.</param>
        /// <param name="configuration">Application configuration.</param>
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRequestLogging(configuration);
            services.AddRequestLoggingIgnorePredicate(new[] { "/swagger/*" });
            services.AddTransient<IHeaderLogMiddleware, CookieHeaderLoggingMiddleware>();
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.TryAddTransient<IDateTimeService, DateTimeService>();
            services.TryAddTransient<IEmailService, EmailService>();
        }
    }
}