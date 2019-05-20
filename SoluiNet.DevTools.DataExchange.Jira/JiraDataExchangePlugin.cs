using RestSharp;
using RestSharp.Authenticators;
using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Core.Tools;
using SoluiNet.DevTools.Core.Tools.String;
using SoluiNet.DevTools.DataExchange.Jira.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SoluiNet.DevTools.DataExchange.Jira
{
    public class JiraDataExchangePlugin : IDataExchangePlugin, IPluginWithSettings, IUtilitiesDevPlugin
    {
        public string Name
        {
            get
            {
                return "JiraDataExchange";
            }
        }

        public string MenuItemLabel
        {
            get { return "JIRA"; }
        }

        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            displayInPluginContainer(new JiraUserControl());
        }

        public List<object> GetData(string entityName, IDictionary<string, object> searchData)
        {
            //throw new NotImplementedException();

            var settings = PluginHelper.GetSettingsAsDictionary(this);

            if (settings == null)
            {
                return new List<object>();
            }

            var client = new RestClient(settings["Default.JiraUrl"].ToString());

            // registration needed
            // https://developer.atlassian.com/cloud/jira/platform/oauth-2-authorization-code-grants-3lo-for-apps/#accesstoken

            if (entityName == "ticket")
            {
                //var request = new RestRequest("rest/api/latest/issue/{issueKey}", Method.GET);
                var request = new RestRequest("rest/api/latest/search", Method.GET);
                //request.AddUrlSegment("issueKey", searchData["issueKey"].ToString());

                foreach (var searchElement in searchData)
                {
                    request.AddParameter(searchElement.Key, searchElement.Value.ToString());
                }

                //request.AddHeader("Authorization", string.Format("Bearer {0}", settings["Default.AccessToken"].ToString()));
                if (!string.IsNullOrEmpty(settings.ContainsKey("Default.JiraAuthentication") ? settings["Default.JiraAuthentication"]?.ToString() : string.Empty))
                {
                    var authenticationMethod = Enum.Parse(typeof(JiraAuthentication), settings["Default.JiraAuthentication"].ToString());

                    switch (authenticationMethod)
                    {
                        case JiraAuthentication.JwtAuthentication:
                            if (!string.IsNullOrEmpty(settings.ContainsKey("Default.AccessToken") ? settings["Default.AccessToken"]?.ToString() : string.Empty))
                            {
                                request.AddHeader("Bearer", settings["Default.AccessToken"].ToString());
                            }
                            break;
                        case JiraAuthentication.BasicAuthentication:
                            if (string.IsNullOrEmpty(settings.ContainsKey("Default.JiraUser") ? settings["Default.JiraUser"]?.ToString() : string.Empty))
                            {
                                throw new Exception("No JIRA user found in settings");
                            }

                            if (string.IsNullOrEmpty(settings.ContainsKey("Default.JiraApiToken") ? settings["Default.JiraApiToken"]?.ToString() : string.Empty))
                            {
                                throw new Exception("No JIRA API token found in settings");
                            }

                            var jiraUser = settings["Default.JiraUser"]?.ToString();
                            var jiraApiToken = settings["Default.JiraApiToken"]?.ToString();

                            //var authenticationValue = string.Format("{0}:{1}", jiraUser, jiraApiToken).ToBase64();

                            //request.AddHeader("Authorization", string.Format("Basic {0}", authenticationValue));
                            client.Authenticator = new HttpBasicAuthenticator(jiraUser, jiraApiToken);
                            break;
                    }
                }
                //request.AddHeader("Accept", "application/json");
                //request.AddHeader("Content-Type", "application/json");


                var response = client.Execute(request);
                var content = response.Content;

                return new List<object> { content };
            }

            return null;
        }

        public List<object> GetData(string whereClause)
        {
            throw new NotImplementedException();
        }

        public object SetData(object identifier, IDictionary<string, object> valueData)
        {
            throw new NotImplementedException();
        }
    }
}
