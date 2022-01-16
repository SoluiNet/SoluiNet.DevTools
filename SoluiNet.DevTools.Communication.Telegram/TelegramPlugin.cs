// <copyright file="TelegramPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Communication.Telegram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using global::Telegram.Bot;
    using RestSharp;
    using RestSharp.Authenticators;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Exceptions;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Plugin.Events;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Plugin;
    using SoluiNet.DevTools.Core.Tools.String;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

    /// <summary>
    /// A plugin which provides data exchange methods for Telegram.
    /// </summary>
    public class TelegramPlugin : IAllowsDataExchange, IContainsSettings, IUtilitiesDevPlugin, IHandlesEvent<IInitializedEvent>,
        ICommunicationReceiver, ICommunicationSender
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get
            {
                return "Telegram";
            }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "Telegram"; }
        }

        /// <inheritdoc/>
        public string CommunicationChannel
        {
            get
            {
                return "Telegram";
            }
        }

        /// <inheritdoc/>
        public ICollection<string> SupportedCommunicationEntities
        {
            get
            {
                return new List<string>() { "Message" };
            }
        }

        /// <summary>
        /// Call this method if the plugin should be displayed.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called for displaying the plugin.</param>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            if (displayInPluginContainer == null)
            {
                throw new ArgumentNullException(nameof(displayInPluginContainer));
            }

            displayInPluginContainer(new TelegramUserControl());
        }

        /// <summary>
        /// Get data from Telegram.
        /// </summary>
        /// <param name="entityName">The type of the message.</param>
        /// <param name="searchData">The search parameters in a dictionary. All parameters will be combined via AND.</param>
        /// <returns>Returns a list of issues that are matching the overgiven parameters.</returns>
        public ICollection<object> GetData(string entityName, IDictionary<string, object> searchData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get data from Telegram.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>Returns a list of issues that are matching the overgiven parameters.</returns>
        public ICollection<object> GetData(string whereClause)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get data by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns a <see cref="object"/> for the requested key.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> until this method has been implemented.</exception>
        public object GetDataByKey(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get data for general purposes.
        /// </summary>
        /// <returns>Returns a dictionary of data for general purposes.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> because this method won't be needed for Telegram data exchange.</exception>
        public IDictionary<string, object> GetGeneralData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle the event.
        /// </summary>
        /// <param name="eventArgs">The event arguments.</param>
        /// <typeparam name="T">The event type.</typeparam>
        public void HandleEvent<T>(Dictionary<string, object> eventArgs)
            where T : IEventType
        {
            if (typeof(T).IsAssignableFrom(typeof(IInitializedEvent)))
            {
                this.Send("Initialized", "@kimiyou88");
            }
        }

        /// <inheritdoc />
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> until the functionality has been implemented.</exception>
        public void Receive(string entity = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> until the functionality has been implemented.</exception>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if the message is null or empty.</exception>
        public async void Send(string message, string entity = "", string receiver = "", Dictionary<string, object> additionalParameters = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(entity))
            {
                entity = "Message";
            }

            var client = new TelegramBotClient(ApplicationContext.Configuration.Settings.GetByKey("Telegram.AccessToken", "Telegram").ToString());

            var telegramMessage = await client.SendTextMessageAsync(receiver, "Telegram plugin initialized").ConfigureAwait(true);
        }

        /// <summary>
        /// Set data in Telegram.
        /// </summary>
        /// <param name="identifier">The identifier of the message.</param>
        /// <param name="valueData">The properties which should be changed.</param>
        /// <returns>Returns the changed issue.</returns>
        public object SetData(object identifier, IDictionary<string, object> valueData)
        {
            throw new NotImplementedException();
        }
    }
}
