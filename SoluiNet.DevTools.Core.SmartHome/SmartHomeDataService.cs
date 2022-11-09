// <copyright file="SmartHomeDataService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.SmartHome
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;
    using NLog;
    using SoluiNet.DevTools.Core.Services;
    using SoluiNet.DevTools.Core.SmartHome.Data;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Plugin;

    /// <summary>
    /// The service to work with smart home data.
    /// </summary>
    public class SmartHomeDataService : ISoluiNetService, IDisposable
    {
        /// <summary>
        /// A list of all observable smart home data plugins.
        /// </summary>
        private ICollection<IObservable<SmartHomeDictionary>> smartHomeDataPlugins;

        /// <summary>
        /// A list of all observers for smart home data plugins.
        /// </summary>
        private IDictionary<IObserver<SmartHomeDictionary>, IDisposable> smartHomeDataObservers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartHomeDataService"/> class.
        /// </summary>
        public SmartHomeDataService()
        {
            this.smartHomeDataPlugins = PluginHelper.GetPlugins<IObservable<SmartHomeDictionary>>();
            this.smartHomeDataObservers = new Dictionary<IObserver<SmartHomeDictionary>, IDisposable>();

            this.InitializeObservers();
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return "SmartHomeData";
            }
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
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the service.
        /// </summary>
        /// <param name="disposing">A value indicating whether the object is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var unsubscriber in this.smartHomeDataObservers)
                {
                    unsubscriber.Value.Dispose();
                }
            }
        }

        /// <summary>
        /// Initialize the observers.
        /// </summary>
        private void InitializeObservers()
        {
            foreach (var plugin in this.smartHomeDataPlugins)
            {
                var observer = new SmartHomeDataObserver();
                observer.NewDataReceived += (sender, eventArgs) =>
                {
                    Logger.Info(JsonConvert.SerializeObject(eventArgs.Data));
                };

                this.smartHomeDataObservers.Add(observer, plugin.Subscribe(observer));
            }
        }
    }
}
