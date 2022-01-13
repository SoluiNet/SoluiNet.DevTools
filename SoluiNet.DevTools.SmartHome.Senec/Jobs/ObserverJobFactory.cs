// <copyright file="ObserverJobFactory.cs" company="SoluiNet">
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
    using Quartz.Spi;
    using SoluiNet.DevTools.Core.SmartHome.Data;

    /// <summary>
    /// The observer job factory.
    /// </summary>
    public sealed class ObserverJobFactory : IJobFactory
    {
        private ICollection<IObserver<SmartHomeDictionary>> observers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObserverJobFactory"/> class.
        /// </summary>
        /// <param name="observers">The observers.</param>
        public ObserverJobFactory(ICollection<IObserver<SmartHomeDictionary>> observers)
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
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            Logger.Debug("Get new observer job");

            return new CallObserversWithNewDataJob(this.observers);
        }

        /// <inheritdoc />
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> since it hasn't been implemented until now.</exception>
        public void ReturnJob(IJob job)
        {
            Logger.Debug("Return observer job");
        }
    }
}
