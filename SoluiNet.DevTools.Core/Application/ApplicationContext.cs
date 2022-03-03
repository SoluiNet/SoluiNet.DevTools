// <copyright file="ApplicationContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Application.Adapter;
    using SoluiNet.DevTools.Core.Exceptions;

    /// <summary>
    /// Defines the application context.
    /// </summary>
    public static class ApplicationContext
    {
        private static Dictionary<string, object> singletons = new Dictionary<string, object>();

        static ApplicationContext()
        {
            SessionValues = new Dictionary<string, object>();
            Configuration = new ApplicationConfigurationAdapter();
            Services = new ServicesAdapter();
        }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public static ISoluiNetApp Application { get; set; }

        /// <summary>
        /// Gets the session values.
        /// </summary>
        public static Dictionary<string, object> SessionValues { get; }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public static ApplicationConfigurationAdapter Configuration { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        public static ServicesAdapter Services { get; }

        /// <summary>
        /// Add a singleton for dependency injection possibilities.
        /// </summary>
        /// <typeparam name="T">The type which will be stored.</typeparam>
        /// <param name="name">The name which should be used to resolve the singleton later.</param>
        /// <param name="instance">The instance of the type which should be used.</param>
        public static void AddSingleton<T>(string name, T instance)
        {
            singletons.Add(name, instance);
        }

        /// <summary>
        /// Resolve the singleton by name.
        /// </summary>
        /// <typeparam name="T">The type which has been stored.</typeparam>
        /// <param name="name">The name which should be used for resolving.</param>
        /// <returns>Returns a <typeparamref name="T"/> which has been stored under the given name.</returns>
        public static T ResolveSingleton<T>(string name)
        {
            if (!singletons.ContainsKey(name))
            {
                throw new SoluiNetException("Singleton with '{0}' and with type '{1}' couldn't be found", name, typeof(T).FullName);
            }

            var instance = singletons[name];

            if (!(instance is T))
            {
                throw new SoluiNetException("Singleton with '{0}' is not available with type '{1}'", name, typeof(T).FullName);
            }

            return (T)singletons[name];
        }
    }
}
