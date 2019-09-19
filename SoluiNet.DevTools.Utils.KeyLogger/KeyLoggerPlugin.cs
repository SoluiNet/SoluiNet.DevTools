// <copyright file="KeyLoggerPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.KeyLogger
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using Quartz;
    using Quartz.Impl;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
    using SoluiNet.DevTools.Utils.KeyLogger.Job;

    /// <summary>
    /// The key logger plugin.
    /// </summary>
    public class KeyLoggerPlugin : IUtilitiesDevPlugin, IRunsBackgroundTask
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "KeyLoggerPlugin"; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "KeyLogger Tools"; }
        }

        /// <summary>
        /// Call this method if the plugin should be displayed.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called for displaying the plugin.</param>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new KeyLoggerUserControl());
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

            var logJob = JobBuilder.Create<LogKeyTask>()
                .WithIdentity("LogKey", "KeyLogger")
                .Build();

            var logTrigger = TriggerBuilder.Create()
                .WithIdentity("LogKey", "KeyLogger")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(logJob, logTrigger);
        }
    }
}
