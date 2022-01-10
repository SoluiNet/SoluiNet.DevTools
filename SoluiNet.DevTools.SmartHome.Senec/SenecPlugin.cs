// <copyright file="SenecPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SmartHome.Senec
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Security.Principal;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.Common;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.SmartHome.Data;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

    /// <summary>
    /// Provides a plugin for the Senec battery storage.
    /// </summary>
    public class SenecPlugin : ISmartHomeUiPlugin, IObservable<SmartHomeDictionary>, IAllowsGenericDataExchange<SmartHomeDictionary>
    {
        /// <summary>
        /// The list of all smart home observers.
        /// </summary>
        private readonly List<IObserver<SmartHomeDictionary>> smartHomeObservers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SenecPlugin"/> class.
        /// </summary>
        public SenecPlugin()
        {
            smartHomeObservers = new List<IObserver<SmartHomeDictionary>>();
        }

        /// <summary>
        /// Gets or sets the first accent colour.
        /// </summary>
        public Color AccentColour1
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the second accent colour.
        /// </summary>
        public Color AccentColour2
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the foreground colour.
        /// </summary>
        public Color ForegroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the background colour.
        /// </summary>
        public Color BackgroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the background accent colour.
        /// </summary>
        public Color BackgroundAccentColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string EntityName
        {
            get { return "SenecBatteryStorage"; }
        }

        /// <summary>
        /// Gets the type definition.
        /// </summary>
        public object TypeDefinition
        {
            get
            {
                return (new
                {
                    Name = string.Empty,

                })
                .GetType();
            }
        }

        /// <summary>
        /// Display the plugin.
        /// </summary>
        /// <param name="mainGrid">The main grid.</param>
        public void Display(Grid mainGrid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get data filtered by where clause.
        /// </summary>
        /// <remarks>
        /// DO NOT USE - this method isn't implemented for this plugin.
        /// </remarks>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>Returns the data which matches the passed where clause.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> since this method won't be needed for this plugin.</exception>
        public ICollection<object> GetData(string whereClause)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get data filtered by entity name and search data.
        /// </summary>
        /// <remarks>
        /// DO NOT USE - this method isn't implemented for this plugin.
        /// </remarks>
        /// <param name="entityName">The entity name.</param>
        /// <param name="searchData">The search data.</param>
        /// <returns>Returns the data which matches the passed entity name and search data.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> since this method won't be needed for this plugin.</exception>
        public ICollection<object> GetData(string entityName, IDictionary<string, object> searchData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get data by key.
        /// </summary>
        /// <remarks>
        /// DO NOT USE - this method isn't implemented for this plugin.
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <returns>Returns the data which matches the key.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> since this method won't be needed for this plugin.</exception>
        public object GetDataByKey(string key)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get general data.
        /// </summary>
        /// <remarks>
        /// DO NOT USE - this method isn't implemented for this plugin.
        /// </remarks>
        /// <returns>Returns a dictionary of general data.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> since this method won't be needed for this plugin.</exception>
        public IDictionary<string, object> GetGeneralData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a dictionary from the SENEC battery storage.
        /// </summary>
        /// <returns>Returns a <see cref="SmartHomeDictionary"/> with data from the SENEC battery storage.</returns>
        public SmartHomeDictionary GetGenericData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a dictionary from the SENEC battery storage for the passed key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns a <see cref="SmartHomeDictionary"/> with data from the SENEC battery storage which matches the passed key.</returns>
        public SmartHomeDictionary GetGenericDataByKey(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set data for the passed identifier.
        /// </summary>
        /// <remarks>
        /// DO NOT USE - this method isn't implemented for this plugin.
        /// </remarks>
        /// <param name="identifier">The identifier.</param>
        /// <param name="valueData">The data value.</param>
        /// <returns>Returns a object which will represent the status of the data saving.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> since this method won't be needed for this plugin.</exception>
        public object SetData(object identifier, IDictionary<string, object> valueData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Subscribe to SENEC data updates.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns>Returns a <see cref="GenericUnsubscriber{TObservers, TObject}"/> object which will allow the caller to call the disposal method.</returns>
        public IDisposable Subscribe(IObserver<SmartHomeDictionary> observer)
        {
            // Check whether observer is already registered. If not, add it
            if (!smartHomeObservers.Contains(observer))
            {
                smartHomeObservers.Add(observer);

                // Provide observer with existing data.
                observer.OnNext(this.GetGenericData());
            }

            return new GenericUnsubscriber<List<IObserver<SmartHomeDictionary>>, SmartHomeDictionary>(this.smartHomeObservers, observer);
        }
    }
}
