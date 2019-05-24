// <copyright file="LogForegroundWindowTask.cs" company="SoluiNet">
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

    /// <summary>
    /// A Job to log the caption of the most foreground window.
    /// </summary>
    public class LogForegroundWindowTask : IJob
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
                this.LogForegroundWindow();
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(string.Format("{0}\r\n{1}", exception.Message, exception.InnerException != null ? exception.InnerException.Message : string.Empty));
            }

            await Task.CompletedTask;
        }

        private void LogForegroundWindow()
        {
            var windowName = TimeTrackingTools.GetTitleOfWindowInForeground();

            this.Logger.Log(LogLevel.Info, windowName);
        }
    }
}
