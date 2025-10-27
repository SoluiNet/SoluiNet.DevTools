// <copyright file="ServiceCollectionExtensions.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Configuration;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
    using SoluiNet.DevTools.Core.TimeTracking.Platform;
    using SoluiNet.DevTools.Core.TimeTracking.Services;

    /// <summary>
    /// Extension methods for configuring time tracking services in dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds time tracking services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureOptions">Optional configuration action.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddTimeTracking(
            this IServiceCollection services,
            Action<TimeTrackingConfiguration>? configureOptions = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Register configuration
            var configuration = new TimeTrackingConfiguration();
            configureOptions?.Invoke(configuration);
            services.AddSingleton<ITimeTrackingConfiguration>(configuration);

            // Register platform provider
            services.AddSingleton<IPlatformProvider>(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<IPlatformProvider>>();
                var provider = PlatformProviderFactory.CreateProvider(logger);
                
                if (provider == null)
                {
                    throw new PlatformNotSupportedException(
                        $"No platform provider available for current OS: {PlatformProviderFactory.GetCurrentPlatformName()}");
                }

                return provider;
            });

            // Register core services (these will be implemented in later tasks)
            services.AddSingleton<IWindowMonitor, WindowMonitor>();
            services.AddSingleton<ITimeTracker, CrossPlatformTimeTracker>();

            return services;
        }

        /// <summary>
        /// Adds time tracking services with a specific configuration instance.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration instance to use.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddTimeTracking(
            this IServiceCollection services,
            ITimeTrackingConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddSingleton(configuration);

            // Register platform provider
            services.AddSingleton<IPlatformProvider>(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<IPlatformProvider>>();
                var provider = PlatformProviderFactory.CreateProvider(logger);
                
                if (provider == null)
                {
                    throw new PlatformNotSupportedException(
                        $"No platform provider available for current OS: {PlatformProviderFactory.GetCurrentPlatformName()}");
                }

                return provider;
            });

            // Register core services (these will be implemented in later tasks)
            services.AddSingleton<IWindowMonitor, WindowMonitor>();
            services.AddSingleton<ITimeTracker, CrossPlatformTimeTracker>();

            return services;
        }

        /// <summary>
        /// Validates that all required time tracking services are registered.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection ValidateTimeTrackingServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Build a temporary service provider to validate registrations
            using var serviceProvider = services.BuildServiceProvider();

            try
            {
                // Validate that all required services can be resolved
                serviceProvider.GetRequiredService<ITimeTrackingConfiguration>();
                serviceProvider.GetRequiredService<IPlatformProvider>();
                serviceProvider.GetRequiredService<IWindowMonitor>();
                serviceProvider.GetRequiredService<ITimeTracker>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Time tracking services are not properly configured. " +
                    "Ensure AddTimeTracking() has been called before ValidateTimeTrackingServices().",
                    ex);
            }

            return services;
        }
    }
}