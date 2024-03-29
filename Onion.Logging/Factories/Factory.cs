﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Onion.Logging
{
    /// <summary>
    /// Service factory base implementation.
    /// </summary>
    public abstract class Factory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory"/> class.
        /// </summary>
        /// <param name="services">Application service provider.</param>
        protected Factory(IServiceProvider services) => _services = services;

        /// <summary>
        /// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T"/> or null if there is no such service.</returns>
        protected T GetService<T>() => _services.GetService<T>();
    }
}