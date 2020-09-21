using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Behaviours;
using System;
using System.Reflection;

namespace Onion.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationInfrastructure(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
    }
}