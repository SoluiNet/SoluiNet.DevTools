// <copyright file="ManageConnectionStrings.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for ManageConnectionStrings.xaml.
    /// </summary>
    public partial class ManageConnectionStrings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManageConnectionStrings"/> class.
        /// </summary>
        public ManageConnectionStrings()
        {
            this.InitializeComponent();
        }

        private void LoadConnectionStrings()
        {
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                this.MainContent.RowDefinitions.Add(new RowDefinition());

                var connectionStringView = new ConnectionString();
                connectionStringView.Value = connectionString.ConnectionString;
                connectionStringView.NameKey = connectionString.Name;
                connectionStringView.ProviderName = connectionString.ProviderName;

                this.MainContent.Children.Add(connectionStringView);
                Grid.SetRow(connectionStringView, this.MainContent.RowDefinitions.Count - 1);
            }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.LoadConnectionStrings();
        }

        private void ReloadConnectionStrings_Click(object sender, RoutedEventArgs e)
        {
            this.MainContent.Children.Clear();
            this.MainContent.RowDefinitions.Clear();

            ConfigurationManager.RefreshSection("connectionStrings");

            this.LoadConnectionStrings();
        }

        private void SaveConnectionStrings_Click(object sender, RoutedEventArgs e)
        {
            var data = string.Empty;
            data += "<connectionStrings>";

            foreach (var item in this.MainContent.Children)
            {
                if (!(item is ConnectionString))
                {
                    continue;
                }

                var connectionString = (ConnectionString)item;

                if (connectionString.ConnectionStringName.Text == "LocalSqlServer")
                {
                    continue;
                }

                data += string.Format(
                    @"<add name=""{0}"" connectionString=""{1}"" providerName=""{2}"" />",
                    connectionString.NameKey,
                    connectionString.Value,
                    connectionString.ProviderName);
            }

            data += "</connectionStrings>";

            File.Move("./ConnectionStrings.config", string.Format("./ConnectionStrings.{0:yyyy-MM-ddTHH-mm-ss}.config", DateTime.Now));

            File.WriteAllText("./ConnectionStrings.config", data);

            ConfigurationManager.RefreshSection("connectionStrings");
        }
    }
}
