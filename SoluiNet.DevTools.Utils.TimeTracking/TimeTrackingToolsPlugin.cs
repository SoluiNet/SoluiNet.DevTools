// <copyright file="TimeTrackingToolsPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using Quartz;
    using Quartz.Impl;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Plugin.Events;
    using SoluiNet.DevTools.Utils.TimeTracking.Job;

    /// <summary>
    /// A plugin which provides utility functions for time tracking.
    /// </summary>
    public class TimeTrackingToolsPlugin : IUtilitiesDevPlugin, IRunsBackgroundTask, IHandlesEvent<IStartupEvent>, IHandlesEvent<IShutdownEvent>
    {
        private static System.Threading.Mutex mutex = null;

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
        public async Task ExecuteBackgroundTask()
        {
            // construct a scheduler factory
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" },
            };
            var factory = new StdSchedulerFactory(props);

            // get a scheduler
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

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

            await scheduler.ScheduleJob(logJob, logTrigger);

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

            await scheduler.ScheduleJob(dbJob, dbTrigger);
        }

        /// <summary>
        /// Call this method if the plugin should be displayed.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called for displaying the plugin.</param>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new TimeTrackingToolsUserControl());
        }

        /// <summary>
        /// Handle the event.
        /// </summary>
        /// <param name="eventArgs">The event arguments.</param>
        /// <typeparam name="T">The event type.</typeparam>
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

                    // we got mutex and can try to obtain a lock by waitone
                    mutex.WaitOne();
                }
                catch
                {
                    // the specified mutex doesn't exist, we should create it
                    mutex = new System.Threading.Mutex(true, "soluinet.devtools_TimeTracking"); // these names need to match.
                }
            }
        }
    }
}
