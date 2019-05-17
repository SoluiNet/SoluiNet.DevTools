using RestSharp;
using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.DataExchange.Jira
{
    public class JiraDataExchangePlugin : IDataExchangePlugin, IPluginWithSettings
    {
        public string Name
        {
            get
            {
                return "JiraDataExchange";
            }
        }

        public List<object> GetData(string whereClause)
        {
            throw new NotImplementedException();
        }

        public List<object> GetData(string entityName, IDictionary<string, object> searchData)
        {
            //throw new NotImplementedException();

            var settings = PluginHelper.GetSettingsAsDictionary(this);

            var client = new RestClient(settings["Default.JiraUrl"].ToString());

            //var request = new RestRequest("rest/api/latest/issue/{issueKey}", Method.GET);
            var request = new RestRequest("rest/api/latest/search", Method.GET);
            //request.AddUrlSegment("issueKey", searchData["issueKey"].ToString());
            
            foreach(var searchElement in searchData)
            {
                request.AddParameter(searchElement.Key, searchElement.Value);
            }

            //request.AddHeader("Authorization", string.Format("Bearer {0}", settings["Default.AccessToken"].ToString()));
            request.AddHeader("Bearer", settings["Default.AccessToken"].ToString());
            request.AddHeader("Accept", "application/json");

            var response = client.Execute(request);
            var content = response.Content;

            return null;
        }

        public object SetData(object identifier, IDictionary<string, object> valueData)
        {
            throw new NotImplementedException();
        }
    }
}
