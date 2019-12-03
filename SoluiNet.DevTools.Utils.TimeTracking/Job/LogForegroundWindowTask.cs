// <copyright file="LogForegroundWindowTask.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.Job
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using NLog;
    using Quartz;

    /// <summary>
    /// A Job to log the caption of the most foreground window.
    /// </summary>
    public class LogForegroundWindowTask : IJob
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
                LogForegroundWindow();
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(string.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}", exception.Message, exception.InnerException != null ? exception.InnerException.Message : string.Empty));
            }

            await Task.CompletedTask.ConfigureAwait(true);
        }

        private static void LogForegroundWindow()
        {
            var windowName = TimeTrackingTools.GetTitleOfWindowInForeground();

            Logger.Log(LogLevel.Info, windowName);
        }
    }
}
