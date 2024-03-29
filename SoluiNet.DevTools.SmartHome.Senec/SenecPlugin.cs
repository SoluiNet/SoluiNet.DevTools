﻿// <copyright file="SenecPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SmartHome.Senec
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
    using System.Windows.Media;
#endif
    using Newtonsoft.Json;
    using NLog;
    using Quartz;
    using Quartz.Impl;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Common;
    using SoluiNet.DevTools.Core.Configuration;
    using SoluiNet.DevTools.Core.Exceptions;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Reference;
    using SoluiNet.DevTools.Core.SmartHome.Data;
    using SoluiNet.DevTools.Core.Tools.Number;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
    using SoluiNet.DevTools.Core.UI.Blazor.Reference;
#if BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.Extensions;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif
    using SoluiNet.DevTools.SmartHome.Senec.Jobs;

    /// <summary>
    /// Provides a plugin for the Senec battery storage.
    /// </summary>
    public class SenecPlugin : ISmartHomeUiPlugin, IObservable<SmartHomeDictionary>, IAllowsGenericDataExchange<SmartHomeDictionary>, IContainsSettings,
        IRunsBackgroundTask, ISupportsCommandLine
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
            Logger.Debug("Construct SENEC plugin\r\nStacktrace: {0}", Environment.StackTrace);

            this.smartHomeObservers = new List<IObserver<SmartHomeDictionary>>();
        }

        /// <summary>
        /// Gets the first accent colour.
        /// </summary>
        public IColour AccentColour1
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(58, 156, 223); }
        }

        /// <summary>
        /// Gets the second accent colour.
        /// </summary>
        public IColour AccentColour2
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(0xFF, 0xFF, 0xFF); }
        }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        public IColour ForegroundColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(0x00, 0x00, 0x00); }
        }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        public IColour BackgroundColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(0xFF, 0xFF, 0xFF); }
        }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        public IColour BackgroundAccentColour
        {
            get { return ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(58, 156, 223); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return "SmartHome.Senec"; }
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

        /// <inheritdoc/>
        public Dictionary<string, ICollection<object>> Resources
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the help text.
        /// </summary>
        public string HelpText
        {
            get
            {
                return @"senec        Get Information about senec";
            }
        }

#if BUILD_FOR_WINDOWS
        /// <summary>
        /// Gets or sets the main grid.
        /// </summary>
        public Grid MainGrid { get; set; }
#endif

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

#if BUILD_FOR_WINDOWS
        /// <summary>
        /// Display the plugin.
        /// </summary>
        /// <param name="mainGrid">The main grid.</param>
        public void Display(Grid mainGrid)
        {
            this.MainGrid = mainGrid;

            var tabControl = mainGrid.GetChildOfType<TabControl>();

            if (tabControl.Name == "MainTabs")
            {
                var tabItem = new TabItem()
                {
                    Header = "SENEC",
                    Name = "Senec_TabItem",
                    Background = new LinearGradientBrush(this.AccentColour1.AsColor(), this.AccentColour2.AsColor(), 0.00),
                    Foreground = new SolidColorBrush(this.ForegroundColour.AsColor()),
                };

                tabControl.SelectionChanged += (sender, eventArgs) =>
                {
                    if (eventArgs.Source is TabControl)
                    {
                        if (tabItem.IsSelected)
                        {
                            tabControl.Background = new SolidColorBrush(this.BackgroundColour.AsColor());
                        }
                    }
                };

                tabControl.Items.Add(tabItem);

                tabItem.Content = new Grid()
                {
                    Name = "Senec_TabItem_Content",
                    Background = new LinearGradientBrush(this.BackgroundAccentColour.AsColor(), this.BackgroundColour.AsColor(), 45.00),
                };

                ((Grid)tabItem.Content).Children.Add(new SenecUserControl());
            }
        }
#endif

        /// <summary>
        /// Runs a background task which will check for new data every 30 seconds.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> which will be executed in the background.</returns>
        public async Task ExecuteBackgroundTask()
        {
            // construct a scheduler factory
            var props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" },
            };
            var factory = new StdSchedulerFactory(props);

            // get a scheduler
            var scheduler = await factory.GetScheduler().ConfigureAwait(true);
            await scheduler.Start().ConfigureAwait(true);

            var observerJob = JobBuilder.Create<CallObserversWithNewDataJob>()
                .WithIdentity("DeliverNewDataToObserversJob", "SmartHome.Senec")
                .Build();

            var observerTrigger = TriggerBuilder.Create()
                .WithIdentity("DeliverNewDataToObservers", "SmartHome.Senec")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(30)
                    .RepeatForever())
                .Build();

            scheduler.JobFactory = new ObserverJobFactory(this.smartHomeObservers);

            await scheduler.ScheduleJob(observerJob, observerTrigger).ConfigureAwait(true);
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
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = client.PostAsync(
                string.Format("{0}/lala.cgi", ApplicationContext.Configuration.Settings.GetByKey("SmartHome.Senec.RemoteAddress", "SmartHome.Senec").ToString()),
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

                Logger.Debug("SmartHome.Senec - Response: {0}", responseString);

                dynamic responseObject = JsonConvert.DeserializeObject(responseString);

                return new SmartHomeDictionary("Senec.PowerValues")
                {
                    { "Power.Battery", this.DecodeSenecValues(responseObject.ENERGY.GUI_BAT_DATA_POWER.Value) },
                    { "Power.Charge", this.DecodeSenecValues(responseObject.ENERGY.GUI_BAT_DATA_FUEL_CHARGE.Value) },
                    { "Power.Grid", this.DecodeSenecValues(responseObject.ENERGY.GUI_GRID_POW.Value) },
                    { "Power.House", this.DecodeSenecValues(responseObject.ENERGY.GUI_HOUSE_POW.Value) },
                    { "Power.Inverter", this.DecodeSenecValues(responseObject.ENERGY.GUI_INVERTER_POWER.Value) },
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

        /// <inheritdoc/>
        public void ConfigureServices(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int RunCommandLine(IDictionary<string, string> arguments)
        {
            throw new NotImplementedException();
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
                    var numberValue = uint.Parse(valueString, System.Globalization.NumberStyles.AllowHexSpecifier);
                    var numberBytes = BitConverter.GetBytes(numberValue);

                    return BitConverter.ToSingle(numberBytes, 0);
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
