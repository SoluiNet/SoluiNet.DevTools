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
using SoluiNet.DevTools.Utils.TimeTracking.Job;

namespace SoluiNet.DevTools.Utils.TimeTracking
{
    public class TimeTrackingToolsPlugin : IUtilitiesDevPlugin, IPluginWithBackgroundTask
    {
        public string Name
        {
            get { return "TimeTrackingToolsPlugin"; }
        }

        public async Task ExecuteBackgroundTask()
        {
            // construct a scheduler factory
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            var factory = new StdSchedulerFactory(props);

            // get a scheduler
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            var job = JobBuilder.Create<LogForegroundWindowTask>()
                .WithIdentity("ForegroundWindowLogger", "TimeTracking")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            var trigger = TriggerBuilder.Create()
                .WithIdentity("LogForegroundWindow", "TimeTracking")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public string MenuItemLabel
        {
            get { return "Time Tracking Tools"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new TimeTrackingToolsUserControl());
        }
    }
}
