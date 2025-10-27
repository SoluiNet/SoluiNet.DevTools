// <copyright file="CrossPlatformTimeTrackingPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using SoluiNet.DevTools.Core.Plugin;
using SoluiNet.DevTools.Core.Plugin.Events;
using SoluiNet.DevTools.Core.TimeTracking.DependencyInjection;
using SoluiNet.DevTools.Core.TimeTracking.Interfaces;
using SoluiNet.DevTools.Core.TimeTracking.Models;

namespace SoluiNet.DevTools.Utils.CrossPlatformTimeTracking
{
    /// <summary>
    /// A cross-platform time tracking plugin for SoluiNet.DevTools.
    /// </summary>
    public class CrossPlatformTimeTrackingPlugin :
        ISupportsCommandLine,
        IRunsBackgroundTask,
        IHandlesEvent<IStartupEvent>,
        IHandlesEvent<IShutdownEvent>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IServiceProvider? serviceProvider;
        private static ITimeTracker? timeTracker;
        private static CancellationTokenSource? cancellationTokenSource;

        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        public string Name => "CrossPlatformTimeTrackingPlugin";

        /// <summary>
        /// Gets the help text for command line usage.
        /// </summary>
        public string HelpText => @"
Time Tracking Commands:
  timetracking start [interval=10] [verbose=false]  - Start time tracking
  timetracking stop                                 - Stop time tracking  
  timetracking status                               - Show time tracking status
  
Examples:
  sndt timetracking start interval=5 verbose=true
  sndt timetracking stop
  sndt timetracking status";

        /// <summary>
        /// Executes the background task for time tracking.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteBackgroundTask()
        {
            try
            {
                Logger.Info("Starting cross-platform time tracking background task");

                // Initialize services if not already done
                if (serviceProvider == null)
                {
                    InitializeServices();
                }

                timeTracker = serviceProvider!.GetRequiredService<ITimeTracker>();

                // Set up event handlers
                timeTracker.UsageDataCaptured += OnUsageDataCaptured;

                cancellationTokenSource = new CancellationTokenSource();

                // Start time tracking
                await timeTracker.StartAsync(cancellationTokenSource.Token);

                Logger.Info("Cross-platform time tracking started successfully");

                // Keep the background task running
                try
                {
                    await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    Logger.Info("Time tracking background task cancelled");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in time tracking background task");
            }
        }

        /// <summary>
        /// Parses and runs the command line arguments.
        /// </summary>
        /// <param name="arguments">The command line arguments.</param>
        /// <returns>The exit code.</returns>
        public int RunCommandLine(IDictionary<string, string> arguments)
        {
            try
            {
                if (arguments == null || !arguments.ContainsKey("action"))
                {
                    Console.WriteLine("Error: No action specified. Use 'timetracking start', 'timetracking stop', or 'timetracking status'");
                    return 1;
                }

                var action = arguments["action"].ToLowerInvariant();

                switch (action)
                {
                    case "start":
                        return HandleStartCommand(arguments);
                    case "stop":
                        return HandleStopCommand();
                    case "status":
                        return HandleStatusCommand();
                    default:
                        Console.WriteLine($"Error: Unknown action '{action}'. Available actions: start, stop, status");
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error handling command line arguments");
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Handles events from the application lifecycle.
        /// </summary>
        /// <param name="eventArgs">The event arguments.</param>
        /// <typeparam name="T">The event type.</typeparam>
        public void HandleEvent<T>(Dictionary<string, object> eventArgs)
            where T : IEventType
        {
            try
            {
                if (typeof(T).IsAssignableFrom(typeof(IStartupEvent)))
                {
                    Logger.Info("Cross-platform time tracking plugin starting up");
                    InitializeServices();
                }
                else if (typeof(T).IsAssignableFrom(typeof(IShutdownEvent)))
                {
                    Logger.Info("Cross-platform time tracking plugin shutting down");
                    
                    // Stop time tracking if running
                    if (timeTracker != null && cancellationTokenSource != null)
                    {
                        try
                        {
                            cancellationTokenSource.Cancel();
                            timeTracker.StopAsync().GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Error stopping time tracking during shutdown");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error handling event in time tracking plugin");
            }
        }

        /// <summary>
        /// Handles the start command.
        /// </summary>
        /// <param name="arguments">The command arguments.</param>
        /// <returns>The exit code.</returns>
        private static int HandleStartCommand(IDictionary<string, string> arguments)
        {
            try
            {
                // Parse interval parameter
                var intervalSeconds = 10;
                if (arguments.ContainsKey("interval") && int.TryParse(arguments["interval"], out var parsedInterval))
                {
                    intervalSeconds = parsedInterval;
                }

                // Parse verbose parameter
                var verbose = arguments.ContainsKey("verbose") && 
                             bool.TryParse(arguments["verbose"], out var parsedVerbose) && parsedVerbose;

                Console.WriteLine($"Starting cross-platform time tracking with {intervalSeconds}s interval...");

                // Initialize services if not already done
                if (serviceProvider == null)
                {
                    InitializeServices(intervalSeconds, verbose);
                }

                timeTracker = serviceProvider!.GetRequiredService<ITimeTracker>();

                // Set up event handlers
                timeTracker.UsageDataCaptured += OnUsageDataCaptured;

                cancellationTokenSource = new CancellationTokenSource();

                // Start time tracking
                var startTask = timeTracker.StartAsync(cancellationTokenSource.Token);
                startTask.GetAwaiter().GetResult();

                Console.WriteLine("Time tracking started successfully.");
                Console.WriteLine("Time tracking is now running in the background.");
                
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error starting time tracking");
                Console.WriteLine($"Error starting time tracking: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Handles the stop command.
        /// </summary>
        /// <returns>The exit code.</returns>
        private static int HandleStopCommand()
        {
            try
            {
                if (timeTracker == null)
                {
                    Console.WriteLine("Time tracking is not running.");
                    return 0;
                }

                var isRunning = timeTracker.IsRunningAsync().GetAwaiter().GetResult();
                if (!isRunning)
                {
                    Console.WriteLine("Time tracking is not running.");
                    return 0;
                }

                Console.WriteLine("Stopping time tracking...");
                
                cancellationTokenSource?.Cancel();
                timeTracker.StopAsync().GetAwaiter().GetResult();

                Console.WriteLine("Time tracking stopped successfully.");
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error stopping time tracking");
                Console.WriteLine($"Error stopping time tracking: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Handles the status command.
        /// </summary>
        /// <returns>The exit code.</returns>
        private static int HandleStatusCommand()
        {
            try
            {
                if (serviceProvider == null)
                {
                    InitializeServices();
                }

                timeTracker ??= serviceProvider!.GetRequiredService<ITimeTracker>();

                var status = timeTracker.GetStatusAsync().GetAwaiter().GetResult();
                var isRunning = timeTracker.IsRunningAsync().GetAwaiter().GetResult();

                Console.WriteLine("=== Cross-Platform Time Tracking Status ===");
                Console.WriteLine($"Status: {(isRunning ? "Running" : "Stopped")}");
                Console.WriteLine($"Platform Provider: {status.PlatformProvider}");
                Console.WriteLine($"Monitoring Interval: {status.MonitoringInterval}");
                
                if (status.StartedAt.HasValue)
                {
                    Console.WriteLine($"Started At: {status.StartedAt:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"Uptime: {status.Uptime}");
                }

                Console.WriteLine($"Data Points Captured: {status.DataPointsCaptured}");

                if (status.LastCapturedWindow != null)
                {
                    Console.WriteLine($"Last Window: {status.LastCapturedWindow}");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error getting time tracking status");
                Console.WriteLine($"Error getting status: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Initializes the dependency injection services.
        /// </summary>
        /// <param name="intervalSeconds">The monitoring interval in seconds.</param>
        /// <param name="verbose">Whether to enable verbose logging.</param>
        private static void InitializeServices(int intervalSeconds = 10, bool verbose = false)
        {
            var services = new ServiceCollection();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(verbose ? Microsoft.Extensions.Logging.LogLevel.Debug : Microsoft.Extensions.Logging.LogLevel.Information);
                
                // Add NLog provider
                builder.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            });

            // Add time tracking services
            services.AddTimeTracking(config =>
            {
                config.MonitoringIntervalSeconds = intervalSeconds;
                config.LogLevel = verbose ? Microsoft.Extensions.Logging.LogLevel.Debug : Microsoft.Extensions.Logging.LogLevel.Information;
                config.EnableAutoStart = false;
            });

            serviceProvider = services.BuildServiceProvider();

            Logger.Info("Cross-platform time tracking services initialized");
        }

        /// <summary>
        /// Handles usage data captured events.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The usage data event arguments.</param>
        private static void OnUsageDataCaptured(object? sender, UsageDataEventArgs e)
        {
            Logger.Debug($"Usage data captured: {e.WindowInfo} for {e.Duration}");
            
            // In a real implementation, this would save to database
            // For now, just log the information
            var timestamp = e.CapturedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            var duration = e.Duration.TotalSeconds.ToString("F1", CultureInfo.InvariantCulture);
            
            Logger.Info($"[{timestamp}] {e.WindowInfo.ProcessName} - {e.WindowInfo.Title} ({duration}s)");
        }
    }
}