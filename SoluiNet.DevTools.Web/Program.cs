// <copyright file="Program.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;
    using NLog;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Web.Application;

    /// <summary>
    /// The service program.
    /// </summary>
    public static class Program
    {
        private static Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        /// <summary>
        /// The main entrance of the application.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        public static void Main()
        {
            try
            {
                ApplicationContext.Application = new WebApplication();

                var servicesToRun = new ServiceBase[]
                {
                    new SoluiNetService(),
                };

                // In interactive and debug mode ?
                if (Environment.UserInteractive)
                {
                    // Simulate the services execution
                    RunInteractiveServices(servicesToRun);
                }
                else
                {
                    // Normal service execution
                    ServiceBase.Run(servicesToRun);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error in SoluiNet.DevTools.Web");
            }
        }

        /// <summary>
        /// Run services in interactive mode.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a service which should be universal. So we use the most common language for information messages - which is English.")]
        private static void RunInteractiveServices(ServiceBase[] servicesToRun)
        {
            Console.WriteLine();
            Console.WriteLine("Start the services in interactive mode.");
            Console.WriteLine();

            // Get the method to invoke on each service to start it
            var onStartMethod = typeof(ServiceBase).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);

            // Start services loop
            foreach (var service in servicesToRun)
            {
                Console.WriteLine("Starting {0} ... ", service.ServiceName);
                onStartMethod?.Invoke(service, new object[] { Array.Empty<string>() });
                Console.WriteLine("{0} started successfully", service.ServiceName);
            }

            // Waiting the end
            Console.WriteLine();
            Console.WriteLine("All services are started.");
            Console.WriteLine();
            Console.WriteLine("Press a key to stop services...");
            Console.ReadKey();
            Console.WriteLine();

            // Get the method to invoke on each service to stop it
            var onStopMethod = typeof(ServiceBase).GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);

            // Stop loop
            foreach (var service in servicesToRun)
            {
                Console.WriteLine("Stopping {0} ... ", service.ServiceName);
                onStopMethod?.Invoke(service, null);
                Console.WriteLine("{0} stopped succesfully", service.ServiceName);
            }

            Console.WriteLine();
            Console.WriteLine("All services are stopped.");

            // Waiting a key press to not return to VS directly
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.Write("=== Press a key to quit ===");
                Console.ReadKey();
            }
        }
    }
}
