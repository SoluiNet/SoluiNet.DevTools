// <copyright file="SmartHomeDataService.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.SmartHome
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Services;
    using SoluiNet.DevTools.Core.SmartHome.Data;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Plugin;

    /// <summary>
    /// The service to work with smart home data.
    /// </summary>
    public class SmartHomeDataService : ISoluiNetService
    {
        private ICollection<IObservable<SmartHomeDictionary>> smartHomeDataPlugins;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartHomeDataService"/> class.
        /// </summary>
        public SmartHomeDataService()
        {
            this.smartHomeDataPlugins = PluginHelper.GetPlugins<IObservable<SmartHomeDictionary>>();

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
        /// Initialize the observers.
        /// </summary>
        private void InitializeObservers()
        {
            foreach (var plugin in this.smartHomeDataPlugins)
            {
                var observer = new SmartHomeDataObserver();

                plugin.Subscribe(observer);
            }
        }
    }
}
