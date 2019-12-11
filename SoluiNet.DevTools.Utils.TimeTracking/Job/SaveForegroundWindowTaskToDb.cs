// <copyright file="SaveForegroundWindowTaskToDb.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.Job
{
    using System;
    using System.Globalization;
    using System.Linq;
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
        private static Logger Logger
        {
            get
            {
                return LogManager.GetLogger("timeTracking");
            }
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "Exceptions which happen in a background task couldn't be brought to front. So log them instead.")]
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                SaveForegroundWindowToDb();
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", exception.Message, exception.InnerException != null ? exception.InnerException.Message : string.Empty));
            }

            await Task.CompletedTask.ConfigureAwait(true);
        }

        private static void SaveForegroundWindowToDb()
        {
            var windowName = TimeTrackingTools.GetTitleOfWindowInForeground();

            var context = new TimeTrackingContext();

            try
            {
                var lowerDayLimit = DateTime.UtcNow.Date;
                var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

                var lastEntry = context.UsageTime
                    .Where(x => x.ApplicationIdentification == windowName && x.StartTime >= lowerDayLimit &&
                                x.StartTime < upperDayLimit).OrderByDescending(x => x.StartTime).FirstOrDefault();

                if (lastEntry == null ||
                    (DateTime.UtcNow - lastEntry.StartTime.ToUniversalTime().AddSeconds(lastEntry.Duration))
                    .TotalSeconds > 15)
                {
                    context.UsageTime.Add(new UsageTime()
                    {
                        StartTime = DateTime.UtcNow,
                        ApplicationIdentification = windowName,
                        Duration = 10,
                    });

                    context.SaveChanges();
                }
                else
                {
                    lastEntry.Duration += 10;

                    context.SaveChanges();
                }
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}
