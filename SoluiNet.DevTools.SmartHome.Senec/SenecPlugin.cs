// <copyright file="SenecPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SmartHome.Senec
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Newtonsoft.Json;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Common;
    using SoluiNet.DevTools.Core.Configuration;
    using SoluiNet.DevTools.Core.Exceptions;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.SmartHome.Data;
    using SoluiNet.DevTools.Core.Tools.Number;
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
            this.smartHomeObservers = new List<IObserver<SmartHomeDictionary>>();
        }

        /// <summary>
        /// Gets the first accent colour.
        /// </summary>
        public Color AccentColour1
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the second accent colour.
        /// </summary>
        public Color AccentColour2
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        public Color ForegroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        public Color BackgroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        public Color BackgroundAccentColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the name.
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
                return new
                {
                    Name = string.Empty,
                }
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
            var client = new HttpClient();

            var jsonObject = new
            {
                ENERGY = new
                {
                    STAT_STATE = string.Empty,
                    STAT_STATE_DECODE = string.Empty,
                    GUI_BAT_DATA_POWER = string.Empty,
                    GUI_INVERTER_POWER = string.Empty,
                    GUI_HOUSE_POW = string.Empty,
                    GUI_GRID_POW = string.Empty,
                    GUI_BAT_DATA_FUEL_CHARGE = string.Empty,
                    GUI_CHARGING_INFO = string.Empty,
                    GUI_BOOSTING_INFO = string.Empty,
                },
                SYS_UPDATE = new
                {
                    UPDATE_AVAILABLE = string.Empty,
                },
            };

            var content = new StringContent(JsonConvert.SerializeObject(jsonObject));
            content.Headers.ContentType = new MediaTypeHeaderValue("Content-Type: application/json; charset=utf-8");

            var response = client.PostAsync(
                ApplicationContext.Configuration.GetByKey("remoteAddress", "SmartHome.Senec").ToString(),
                content)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                dynamic responseObject = JsonConvert.DeserializeObject(responseString);

                return new SmartHomeDictionary("Senec.PowerValues")
                {
                    { "Power.Grid", this.DecodeSenecValues(responseObject.ENERGY.GUI_GRID_POW) },
                };
            }

            return null;
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
            if (!this.smartHomeObservers.Contains(observer))
            {
                this.smartHomeObservers.Add(observer);

                // Provide observer with existing data.
                observer.OnNext(this.GetGenericData());
            }

            return new GenericUnsubscriber<List<IObserver<SmartHomeDictionary>>, SmartHomeDictionary>(this.smartHomeObservers, observer);
        }

        /// <summary>
        /// Decode SENEC response values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns an <see cref="object"/> with the value of the passed string. May need casting to right data type in the calling method.</returns>
        private object DecodeSenecValues(string value)
        {
            var prefix = value.Substring(0, 3);
            var valueString = value.Substring(3, value.Length - 3);
            var number = valueString.FromHexValue();

            switch (prefix)
            {
                case "fl_":
                    var sign = (number & 0x80000000) == 0x80000000 ? -1 : 1;
                    var exponent = ((number >> 23) & 0xff) - 127;
                    var mantissa = ((number & 0x7fffff) + 0x800000).ToString();

                    double result = 0;

                    for (var i = 0; i < mantissa.Length; i++)
                    {
                        result += mantissa[i] == '1' ? Math.Pow(2, exponent) : 0;
                        exponent--;
                    }

                    return sign * result;
                case "u1_":
                case "u8_":
                    if (number < 10)
                    {
                        return string.Format("0{0}", number);
                    }

                    break;
                case "u3_":
                case "u6_":
                    return number;
                case "i1_":
                    if (number < 10)
                    {
                        return string.Format("0{0}", number);
                    }

                    if ((number & 0x8000) > 0)
                    {
                        return number - 0x10000;
                    }

                    break;
                case "i3_":
                    if (number < 10)
                    {
                        return string.Format("0{0}", number);
                    }

                    if (Math.Abs(number & 0x80000000) > 0)
                    {
                        return number - 0x100000000;
                    }

                    break;
                case "i8_":
                    if (number < 10)
                    {
                        return string.Format("0{0}", number);
                    }

                    if ((number & 0x80) > 0)
                    {
                        return number - 0x100;
                    }

                    break;
                case "ch_":
                case "st_":
                    return valueString;
                case "er_":
                    throw new SoluiNetPluginException(
                        string.Format("Error in SENEC value decoding: {0}", value),
                        this);
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }

            return null;
        }
    }
}
