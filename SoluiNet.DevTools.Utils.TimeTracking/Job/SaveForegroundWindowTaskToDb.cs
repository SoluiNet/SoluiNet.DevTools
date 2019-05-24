// <copyright file="SaveForegroundWindowTaskToDb.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NLog;
    using Quartz;
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;

    /// <summary>
    /// A Job to save the caption of the most foreground window to a database.
    /// </summary>
    public class SaveForegroundWindowTaskToDb : IJob
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        private Logger Logger
        {
            get
            {
                return LogManager.GetLogger("timeTracking");
            }
        }

        /// <inheritdoc/>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                this.SaveForegroundWindowToDb();
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(string.Format("{0}\r\n{1}", exception.Message, exception.InnerException != null ? exception.InnerException.Message : string.Empty));
            }

            await Task.CompletedTask;
        }

        private void SaveForegroundWindowToDb()
        {
            var windowName = TimeTrackingTools.GetTitleOfWindowInForeground();

            var context = new TimeTrackingContext();

            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            var lastEntry = context.UsageTime.Where(x => x.ApplicationIdentification == windowName && x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).OrderByDescending(x => x.StartTime).FirstOrDefault();

            if (lastEntry == null || (DateTime.UtcNow - lastEntry.StartTime.ToUniversalTime().AddSeconds(lastEntry.Duration)).TotalSeconds > 15)
            {
                context.UsageTime.Add(new UsageTime() { StartTime = DateTime.UtcNow, ApplicationIdentification = windowName, Duration = 10 });

                context.SaveChanges();
            }
            else
            {
                lastEntry.Duration += 10;

                context.SaveChanges();
            }
        }
    }
}
