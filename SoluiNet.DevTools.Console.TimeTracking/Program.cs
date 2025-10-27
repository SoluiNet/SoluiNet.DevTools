// <copyright file="Program.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Console.TimeTracking
{
    using System;
    using System.CommandLine;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.DependencyInjection;
    using SoluiNet.DevTools.Core.TimeTracking.Interfaces;

    /// <summary>
    /// Main program class for the time tracking console application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Exit code.</returns>
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Cross-platform time tracking console application");

            // Add commands (these will be implemented in later tasks)
            var startCommand = new Command("start", "Start time tracking");
            var stopCommand = new Command("stop", "Stop time tracking");
            var statusCommand = new Command("status", "Show time tracking status");

            rootCommand.AddCommand(startCommand);
            rootCommand.AddCommand(stopCommand);
            rootCommand.AddCommand(statusCommand);

            // Set up handlers
            startCommand.SetHandler(async () => await HandleStartCommand());
            stopCommand.SetHandler(async () => await HandleStopCommand());
            statusCommand.SetHandler(async () => await HandleStatusCommand());

            return await rootCommand.InvokeAsync(args);
        }

        /// <summary>
        /// Handles the start command.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task HandleStartCommand()
        {
            Console.WriteLine("Starting time tracking...");

            try
            {
                using var host = CreateHost();
                var timeTracker = host.Services.GetRequiredService<ITimeTracker>();
                
                await timeTracker.StartAsync();
                Console.WriteLine("Time tracking started successfully.");
                
                // For demonstration, run for a short time
                Console.WriteLine("Running for 10 seconds for demonstration...");
                await Task.Delay(TimeSpan.FromSeconds(10));
                
                await timeTracker.StopAsync();
                Console.WriteLine("Time tracking stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting time tracking: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the stop command.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task HandleStopCommand()
        {
            Console.WriteLine("Stop command - not yet implemented in this task");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Handles the status command.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task HandleStatusCommand()
        {
            Console.WriteLine("Checking time tracking status...");

            try
            {
                using var host = CreateHost();
                var timeTracker = host.Services.GetRequiredService<ITimeTracker>();
                
                var status = await timeTracker.GetStatusAsync();
                Console.WriteLine($"Status: {status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting status: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates and configures the host.
        /// </summary>
        /// <returns>The configured host.</returns>
        private static IHost CreateHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(LogLevel.Information);
                    });

                    services.AddTimeTracking(config =>
                    {
                        config.MonitoringIntervalSeconds = 5; // 5 seconds for testing
                        config.LogLevel = LogLevel.Information;
                    });
                })
                .Build();
        }
    }
}