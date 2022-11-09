// <copyright file="EMailPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Communication.EMail
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using MailKit.Net.Smtp;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
#endif
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MimeKit;
    using NLog;
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
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
#if BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif

    /// <summary>
    /// A plugin which provides data exchange methods for E-Mails.
    /// </summary>
    public class EMailPlugin : IAllowsDataExchange, IContainsSettings, IUtilitiesDevPlugin, IHandlesEvent<IInitializedEvent>,
        ICommunicationReceiver, ICommunicationSender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EMailPlugin"/> class.
        /// </summary>
        public EMailPlugin()
        {
        }

        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get
            {
                return "EMail";
            }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "EMail"; }
        }

        /// <inheritdoc/>
        public string CommunicationChannel
        {
            get
            {
                return "EMail";
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

        /// <inheritdoc/>
        public Dictionary<string, ICollection<object>> Resources
        {
            get
            {
                return null;
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

#if BUILD_FOR_WINDOWS
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

            displayInPluginContainer(new EMailUserControl());
        }
#endif

        /// <summary>
        /// Get data from E-Mail.
        /// </summary>
        /// <param name="entityName">The type of the message.</param>
        /// <param name="searchData">The search parameters in a dictionary. All parameters will be combined via AND.</param>
        /// <returns>Returns a list of issues that are matching the overgiven parameters.</returns>
        public ICollection<object> GetData(string entityName, IDictionary<string, object> searchData)
        {
            return null;
        }

        /// <summary>
        /// Get data from E-Mail.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>Returns a list of issues that are matching the overgiven parameters.</returns>
        public ICollection<object> GetData(string whereClause)
        {
            return null;
        }

        /// <summary>
        /// Get data by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns a <see cref="object"/> for the requested key.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> until this method has been implemented.</exception>
        public object GetDataByKey(string key)
        {
            return null;
        }

        /// <summary>
        /// Get data for general purposes.
        /// </summary>
        /// <returns>Returns a dictionary of data for general purposes.</returns>
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> because this method won't be needed for Telegram data exchange.</exception>
        public IDictionary<string, object> GetGeneralData()
        {
            return null;
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
            }
        }

        /// <inheritdoc />
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> until the functionality has been implemented.</exception>
        public void Receive(string entity = "")
        {
        }

        /// <inheritdoc />
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> until the functionality has been implemented.</exception>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> if the message is null or empty.</exception>
        public void Send(string message, string entity = "", string receiver = "", Dictionary<string, object> additionalParameters = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(entity))
            {
                entity = "Message";
            }

            var mailMessage = new MimeMessage();

            try
            {
                var sender = ApplicationContext.Configuration.PluginConfiguration.GetByKey("EMailSender").ToString();

                mailMessage.From.Add(
                    new MailboxAddress(
                        string.IsNullOrEmpty(sender) ? "solui.net" : sender,
                        ApplicationContext.Configuration.PluginConfiguration.GetByKey("EMailSenderAddress").ToString()));
                mailMessage.To.Add(
                    new MailboxAddress(
                        receiver,
                        receiver));

                mailMessage.Subject = additionalParameters != null && additionalParameters.ContainsKey("Subject")
                        ? additionalParameters["Subject"].ToString()
                        : "Info";

                mailMessage.Body = new TextPart("plain")
                {
                    Text = message,
                };

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Connect(
                        ApplicationContext.Configuration.PluginConfiguration.GetByKey("EMailSender").ToString(),
                        Convert.ToInt32(ApplicationContext.Configuration.PluginConfiguration.GetByKey("EMailPort"), CultureInfo.InvariantCulture),
                        true);

                    smtpClient.Authenticate(
                        ApplicationContext.Configuration.PluginConfiguration.GetByKey("EMailSenderAddress").ToString(),
                        ApplicationContext.Configuration.PluginConfiguration.GetByKey("EMailSenderPassword").ToString());

                    smtpClient.Send(mailMessage);

                    smtpClient.Disconnect(true);
                }
            }
            finally
            {
                mailMessage.Dispose();
            }
        }

        /// <summary>
        /// Set data in E-Mail.
        /// </summary>
        /// <param name="identifier">The identifier of the message.</param>
        /// <param name="valueData">The properties which should be changed.</param>
        /// <returns>Returns the changed issue.</returns>
        public object SetData(object identifier, IDictionary<string, object> valueData)
        {
            return null;
        }

        /// <inheritdoc/>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        /// <inheritdoc/>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
        }
    }
}
