using System;
using System.Reflection;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}