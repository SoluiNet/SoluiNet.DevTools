// <copyright file="Office365ExchangeTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Office365.Exchange
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Exchange.WebServices.Data;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Plugin;

    /// <summary>
    /// Provides methods to work with Office365 Exchange.
    /// </summary>
    public static class Office365ExchangeTools
    {
        /// <summary>
        /// Get mails from exchange server.
        /// </summary>
        /// <returns>Returns a list of mails.</returns>#
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a service which should be universal. So we use the most common language for information messages - which is English.")]
        public static ICollection<string> GetMails()
        {
            var service = Connect();
            var emailList = new List<string>();

            // Prepare separate class for writing email to the database
            try
            {
                Console.WriteLine("Reading mail");

                // Read 100 mails
                foreach (EmailMessage email in service.FindItems(WellKnownFolderName.Inbox, new ItemView(100)))
                {
                    emailList.Add(email.Subject);
                }

                Console.WriteLine("Exiting");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error has occured. \n:" + e.Message);
            }

            return emailList;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a service which should be universal. So we use the most common language for information messages - which is English.")]
        private static ExchangeService Connect()
        {
            var settings = PluginHelper.GetSettingsAsDictionary(PluginHelper.GetPluginByName<Office365ExchangePlugin>("Office365ExchangePlugin"));

            ExchangeService service;

            try
            {
                Console.WriteLine("Registering Exchange connection");

                service = new ExchangeService
                {
                    Credentials = new WebCredentials(settings["ExchangeUser"].ToString(), settings["ExchangePassword"].ToString()),
                };
            }
            catch
            {
                Console.WriteLine("new ExchangeService failed. Press enter to exit:");
                return null;
            }

            // This is the office365 webservice URL
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

            return service;
        }
    }
}
