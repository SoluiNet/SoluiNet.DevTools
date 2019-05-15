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
