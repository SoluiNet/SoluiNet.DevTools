﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Quartz;

namespace SoluiNet.DevTools.Utils.TimeTracking.Job
{
    public class LogForegroundWindowTask : IJob
    {
        private Logger Logger
        {
            get
            {
                return LogManager.GetLogger("timeTracking");
            }
        }

        private void LogForegroundWindow()
        {
            var windowName = TimeTrackingTools.GetTitleOfWindowInForeground();

            Logger.Log(LogLevel.Info, windowName);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                LogForegroundWindow();
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(exception.Message);
            }

            await Task.CompletedTask;
        }
    }
}
