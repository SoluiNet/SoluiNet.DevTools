﻿// <copyright file="TimeTrackingToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
#endif
    using Quartz;
    using Quartz.Impl;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Plugin.Events;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
#if BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.General;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;
    using SoluiNet.DevTools.Utils.TimeTracking.Job;

    /// <summary>
    /// A plugin which provides utility functions for time tracking.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Will be used from outside sources")]
    public class TimeTrackingToolsPlugin :
        SoluiNet.DevTools.Core.UI.Blazor.Plugin.IUtilitiesDevPlugin,
#if BUILD_FOR_WINDOWS
        SoluiNet.DevTools.Core.UI.WPF.Plugin.IUtilitiesDevPlugin,
#endif
        IRunsBackgroundTask,
        IHandlesEvent<IStartupEvent>,
        IHandlesEvent<IShutdownEvent>,
        IHandlesEvent<IApplicationStartedEvent>
    {
        private static System.Threading.Mutex mutex;

        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "TimeTrackingToolsPlugin"; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "Time Tracking Tools"; }
        }

        /// <inheritdoc/>
        public Dictionary<string, ICollection<object>> Resources
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public async Task ExecuteBackgroundTask()
        {
            // construct a scheduler factory
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" },
            };
            var factory = new StdSchedulerFactory(props);

            // get a scheduler
            var scheduler = await factory.GetScheduler().ConfigureAwait(true);
            await scheduler.Start().ConfigureAwait(true);

            var logJob = JobBuilder.Create<LogForegroundWindowTask>()
                .WithIdentity("ForegroundWindowLogger", "TimeTracking")
                .Build();

            var logTrigger = TriggerBuilder.Create()
                .WithIdentity("LogForegroundWindow", "TimeTracking")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(logJob, logTrigger).ConfigureAwait(true);

            var dbJob = JobBuilder.Create<SaveForegroundWindowTaskToDb>()
                .WithIdentity("ForegroundWindowDbPersister", "TimeTracking")
                .Build();

            var dbTrigger = TriggerBuilder.Create()
                .WithIdentity("PersistForegroundWindowToDb", "TimeTracking")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(dbJob, dbTrigger).ConfigureAwait(true);
        }

#if BUILD_FOR_WINDOWS
        /// <summary>
        /// Call this method if the plugin should be displayed.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called for displaying the plugin.</param>
        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:DisposeObjectsBeforeLosingScope",
            Justification = "Current calling methods in MainWindow.xaml already call a Dispose if possible.")]
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            if (displayInPluginContainer == null)
            {
                throw new ArgumentNullException(nameof(displayInPluginContainer));
            }

            displayInPluginContainer(new TimeTrackingToolsUserControl());
        }
#endif

        /// <summary>
        /// Handle the event.
        /// </summary>
        /// <param name="eventArgs">The event arguments.</param>
        /// <typeparam name="T">The event type.</typeparam>
        [SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "The catch is needed for the creation of the mutex.")]
        public void HandleEvent<T>(Dictionary<string, object> eventArgs)
            where T : IEventType
        {
            if (typeof(T).IsAssignableFrom(typeof(IShutdownEvent)))
            {
                mutex.ReleaseMutex();
            }
            else if (typeof(T).IsAssignableFrom(typeof(IStartupEvent)))
            {
                // taken from: https://stackoverflow.com/questions/2186747/how-can-i-create-a-system-mutex-in-c-sharp
                try
                {
                    mutex = System.Threading.Mutex.OpenExisting("soluinet.devtools_TimeTracking");

                    // we got mutex and can try to obtain a lock by WaitOne
                    mutex.WaitOne();
                }
                catch
                {
                    // the specified mutex doesn't exist, we should create it
                    mutex = new System.Threading.Mutex(true, "soluinet.devtools_TimeTracking"); // these names need to match.
                }
            }
            else if (typeof(T).IsAssignableFrom(typeof(IApplicationStartedEvent)))
            {
                var context = new TimeTrackingContext();

                try
                {
                    var currentDateTime = DateTime.UtcNow;
                    var currentDate = currentDateTime.Date;

                    var lastUsageTime = context.UsageTime
                        .Where(x => x.StartTime > currentDate)
                        .OrderByDescending(x => x.StartTime).FirstOrDefault();

                    if (lastUsageTime == null ||
                        !((DateTime.UtcNow - lastUsageTime.StartTime.AddSeconds(lastUsageTime.Duration)).TotalSeconds > 15))
                    {
                        return;
                    }

#if BUILD_FOR_WINDOWS
                    var usageTimeName = Prompt.ShowDialog(
                        Properties.Resources.DescriptionForMeantime,
                        Properties.Resources.MeantimeDescriptionTitle);
#else
                    // todo: implement query for mean time on blazor UI
                    var usageTimeName = "UNKNOWN";
#endif

                    context.UsageTime.Add(new UsageTime()
                    {
                        StartTime = lastUsageTime.StartTime.AddSeconds(lastUsageTime.Duration),
                        ApplicationIdentification = usageTimeName,
                        Duration = Convert.ToInt32((currentDateTime - lastUsageTime.StartTime.AddSeconds(lastUsageTime.Duration)).TotalSeconds),
                    });

                    context.SaveChanges();
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        /// <inheritdoc/>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }
}
