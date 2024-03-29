﻿// <copyright file="JiraDataExchangePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.DataExchange.Jira
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
#endif
    using RestSharp;
    using RestSharp.Authenticators;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Exceptions;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Plugin;
    using SoluiNet.DevTools.Core.Tools.String;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
#if BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;
#endif
    using SoluiNet.DevTools.DataExchange.Jira.Enums;

    /// <summary>
    /// A plugin which provides data exchange methods for Atlassian JIRA.
    /// </summary>
    public class JiraDataExchangePlugin : IAllowsDataExchange, IContainsSettings, IUtilitiesDevPlugin
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get
            {
                return "JiraDataExchange";
            }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "JIRA"; }
        }

        /// <inheritdoc/>
        public Dictionary<string, ICollection<object>> Resources
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            throw new NotImplementedException();
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

            displayInPluginContainer(new JiraUserControl());
        }
#endif

        /// <summary>
        /// Get data from JIRA.
        /// </summary>
        /// <param name="entityName">The type of the issue (bug, feature, ...)</param>
        /// <param name="searchData">The search parameters in a dictionary. All parameters will be combined via AND.</param>
        /// <returns>Returns a list of issues that are matching the overgiven parameters.</returns>
        public ICollection<object> GetData(string entityName, IDictionary<string, object> searchData)
        {
            if (searchData == null)
            {
                throw new ArgumentNullException(nameof(searchData));
            }

            // throw new NotImplementedException();
            var settings = PluginHelper.GetSettingsAsDictionary(this);

            if (settings == null)
            {
                return new List<object>();
            }

            RestClient client = null;

            try
            {
                client = new RestClient(settings["Default.JiraUrl"].ToString());

                // registration needed
                // https://developer.atlassian.com/cloud/jira/platform/oauth-2-authorization-code-grants-3lo-for-apps/#accesstoken
                if (entityName == "ticket")
                {
                    // var request = new RestRequest("rest/api/latest/issue/{issueKey}", Method.GET);
                    var request = new RestRequest("rest/api/latest/search", Method.Get);

                    // request.AddUrlSegment("issueKey", searchData["issueKey"].ToString());
                    foreach (var searchElement in searchData)
                    {
                        request.AddParameter(searchElement.Key, searchElement.Value.ToString());
                    }

                    // request.AddHeader("Authorization", string.Format("Bearer {0}", settings["Default.AccessToken"].ToString()));
                    if (!string.IsNullOrEmpty(settings.ContainsKey("Default.JiraAuthentication") ? settings["Default.JiraAuthentication"]?.ToString() : string.Empty))
                    {
                        var authenticationMethod = Enum.Parse(typeof(JiraAuthentication), settings["Default.JiraAuthentication"].ToString());

                        switch (authenticationMethod)
                        {
                            case JiraAuthentication.JwtAuthentication:
                                if (settings.ContainsKey("Default.AccessToken") && !string.IsNullOrEmpty(settings["Default.AccessToken"]?.ToString()))
                                {
                                    request.AddHeader("Bearer", settings["Default.AccessToken"].ToString());
                                }

                                break;
                            case JiraAuthentication.BasicAuthentication:
                                if (settings.ContainsKey("Default.JiraUser") && string.IsNullOrEmpty(settings["Default.JiraUser"]?.ToString()))
                                {
                                    throw new SoluiNetPluginException("No JIRA user found in settings", this);
                                }

                                if (settings.ContainsKey("Default.JiraApiToken") && string.IsNullOrEmpty(settings["Default.JiraApiToken"]?.ToString()))
                                {
                                    throw new SoluiNetPluginException("No JIRA API token found in settings", this);
                                }

                                var jiraUser = settings["Default.JiraUser"]?.ToString();
                                var jiraApiToken = settings["Default.JiraApiToken"]?.ToString();

                                // var authenticationValue = string.Format("{0}:{1}", jiraUser, jiraApiToken).ToBase64();

                                // request.AddHeader("Authorization", string.Format("Basic {0}", authenticationValue));
                                client.Authenticator = new HttpBasicAuthenticator(jiraUser, jiraApiToken);
                                break;
                        }
                    }

                    // request.AddHeader("Accept", "application/json");
                    // request.AddHeader("Content-Type", "application/json");
                    var response = client.ExecuteAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
                    var content = response.Content;

                    return new List<object> { content };
                }
            }
            finally
            {
                if (client != null)
                {
                    if (client is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get data from JIRA.
        /// </summary>
        /// <param name="whereClause">The JQL-statement.</param>
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
        /// <exception cref="NotImplementedException">Throws a <see cref="NotImplementedException"/> because this method won't be needed for JIRA data exchange.</exception>
        public IDictionary<string, object> GetGeneralData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set data in JIRA.
        /// </summary>
        /// <param name="identifier">The identifier of the issue.</param>
        /// <param name="valueData">The properties which should be changed.</param>
        /// <returns>Returns the changed issue.</returns>
        public object SetData(object identifier, IDictionary<string, object> valueData)
        {
            throw new NotImplementedException();
        }
    }
}
