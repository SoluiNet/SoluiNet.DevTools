// <copyright file="CallObserversWithNewDataJob.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SmartHome.Senec.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NLog;
    using Quartz;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.SmartHome.Data;

    /// <summary>
    /// A job that will call all observers and deliver new data.
    /// </summary>
    public class CallObserversWithNewDataJob : IJob
    {
        private ICollection<IObserver<SmartHomeDictionary>> observers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallObserversWithNewDataJob"/> class.
        /// </summary>
        /// <param name="observers">The smart home data observers.</param>
        public CallObserversWithNewDataJob(ICollection<IObserver<SmartHomeDictionary>> observers)
        {
            this.observers = observers;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private static Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        /// <inheritdoc />
        public async Task Execute(IJobExecutionContext context)
        {
            Logger.Debug("Execute observer job");

            var senecPlugin = ApplicationContext.Application.Plugins.FirstOrDefault(x => x.Name == "SmartHome.Senec") as SenecPlugin;

            var data = senecPlugin.GetGenericData();

            Logger.Debug("Call observers.");

            foreach (var observer in this.observers)
            {
                observer.OnNext(data);
            }

            await Task.CompletedTask.ConfigureAwait(true);
        }
    }
}
