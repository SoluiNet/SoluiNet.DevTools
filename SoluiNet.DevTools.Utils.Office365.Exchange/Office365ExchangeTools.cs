﻿// <copyright file="Office365ExchangeTools.cs" company="SoluiNet">
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

    public static class Office365ExchangeTools
    {
        public static List<string> GetMails()
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

        private static ExchangeService Connect()
        {
            ExchangeService service;

            try
            {
                Console.WriteLine("Registering Exchange connection");

                service = new ExchangeService
                {
                    Credentials = new WebCredentials("yourmailbox@email.com", "*******"),
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