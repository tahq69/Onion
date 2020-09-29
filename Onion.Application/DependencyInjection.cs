using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Behaviours;

namespace Onion.Application
{
    /// <summary>
    /// Application business logic DI.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds all required services for application business logic module.
        /// </summary>
        /// <param name="services">DI service.</param>
        /// <param name="configuration">Application configuration.</param>
        public static void AddApplicationInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
    }
}