using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Quartz;
using SoluiNet.DevTools.Utils.TimeTracking.Entities;

namespace SoluiNet.DevTools.Utils.TimeTracking.Job
{
    public class SaveForegroundWindowTaskToDb : IJob
    {
        private Logger Logger
        {
            get
            {
                return LogManager.GetLogger("timeTracking");
            }
        }

        private void SaveForegroundWindowToDb()
        {
            var windowName = TimeTrackingTools.GetTitleOfWindowInForeground();

            var context = new TimeTrackingContext();

            context.UsageTimes.Add(new UsageTime() { StartTime = DateTime.UtcNow, ApplicationIdentification = windowName, Duration = 0 });

            context.SaveChanges();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                SaveForegroundWindowToDb();
            }
            catch(Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(exception.Message);
            }

            await Task.CompletedTask;
        }
    }
}
