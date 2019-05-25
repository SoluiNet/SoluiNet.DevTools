// <copyright file="ConnectionString.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
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
    /// Interaction logic for ConnectionString.xaml.
    /// </summary>
    public partial class ConnectionString : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionString"/> class.
        /// </summary>
        public ConnectionString()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value
        {
            get { return this.ConnectionStringValue.Text; }
            set { this.ConnectionStringValue.Text = value; }
        }

        /// <summary>
        /// Gets or sets the connection string name.
        /// </summary>
        public string NameKey
        {
            get { return this.ConnectionStringName.Text; }
            set { this.ConnectionStringName.Text = value; }
        }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        public string Environment
        {
            get { return this.NameKey.Contains(".") ? this.NameKey.Substring(this.NameKey.LastIndexOf(".", StringComparison.InvariantCulture) + 1) : "Default"; }
        }

        private bool Expanded { get; set; }

        private void AdditionalInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Expanded)
            {
                this.Expanded = true;

                this.AdditionalInfo.Content = "^";
                this.Height = 200;

                #region provider name
                var providerNameTextBox = new TextBox()
                {
                    Margin = new Thickness(10, 45, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "ProviderName",
                    Width = 200,
                };

                providerNameTextBox.Text = this.ProviderName;
                providerNameTextBox.TextChanged += (o, args) => { this.ProviderName = ((TextBox)o).Text; };

                this.ContentGrid.Children.Add(providerNameTextBox);
                #endregion provider name

                #region environment
                var environmentTextBox = new TextBox()
                {
                    Margin = new Thickness(215, 45, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Environment",
                    Width = 200,
                };

                environmentTextBox.Text = this.Environment;

                this.ContentGrid.Children.Add(environmentTextBox);
                #endregion

                #region server
                var serverTextBox = new TextBox()
                {
                    Margin = new Thickness(10, 70, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Server",
                    Width = 200,
                };

                var dataSourceRegex = new Regex("data source=(.+?);", RegexOptions.IgnoreCase);

                var dataSourceMatch = dataSourceRegex.Match(this.Value);

                if (dataSourceMatch.Success)
                {
                    serverTextBox.Text = dataSourceMatch.Groups[1].Value;
                }

                this.ContentGrid.Children.Add(serverTextBox);
                #endregion

                #region database
                var databaseTextBox = new TextBox()
                {
                    Margin = new Thickness(215, 70, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Database",
                    Width = 200,
                };

                var databaseRegex = new Regex("initial catalog=(.+?);", RegexOptions.IgnoreCase);

                var databaseMatch = databaseRegex.Match(this.Value);

                if (databaseMatch.Success)
                {
                    databaseTextBox.Text = databaseMatch.Groups[1].Value;
                }

                this.ContentGrid.Children.Add(databaseTextBox);
                #endregion

                #region username
                var usernameTextBox = new TextBox()
                {
                    Margin = new Thickness(10, 95, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Username",
                    Width = 200,
                };

                var usernameRegex = new Regex("user id=(.+?);", RegexOptions.IgnoreCase);

                var usernameMatch = usernameRegex.Match(this.Value);

                if (usernameMatch.Success)
                {
                    usernameTextBox.Text = usernameMatch.Groups[1].Value;
                }

                this.ContentGrid.Children.Add(usernameTextBox);
                #endregion

                #region password
                var passwordTextBox = new TextBox()
                {
                    Margin = new Thickness(215, 95, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Password",
                    Width = 200,
                };

                var passwordRegex = new Regex("password=(.+?);", RegexOptions.IgnoreCase);

                var passwordMatch = passwordRegex.Match(this.Value);

                if (passwordMatch.Success)
                {
                    passwordTextBox.Text = passwordMatch.Groups[1].Value;
                }

                this.ContentGrid.Children.Add(passwordTextBox);
                #endregion

                #region use windows authentication
                var useWindowsAuthenticationTextBox = new CheckBox()
                {
                    Margin = new Thickness(10, 120, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "UseWindowsSecurity",
                    Width = 200,
                };

                var useWindowsAuthenticationRegex = new Regex("integrated security=(.+?);", RegexOptions.IgnoreCase);

                var useWindowsAuthenticationMatch = useWindowsAuthenticationRegex.Match(this.Value);

                if (useWindowsAuthenticationMatch.Success)
                {
                    useWindowsAuthenticationTextBox.IsChecked = useWindowsAuthenticationMatch.Groups[1].Value == "SSPI" || Convert.ToBoolean(useWindowsAuthenticationMatch.Groups[1].Value);
                }

                this.ContentGrid.Children.Add(useWindowsAuthenticationTextBox);
                #endregion

                #region calculate connectionstring
                var calculateConnectionStringButton = new Button()
                {
                    Margin = new Thickness(420, 45, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "CalculateConnectionString",
                    Width = 200,
                };

                calculateConnectionStringButton.Content = "Calculate ConnectionString";
                calculateConnectionStringButton.Click += (o, args) =>
                {
                    if (this.ProviderName == "System.Data.SqlClient")
                    {
                        var connectionStringBuilder = new SqlConnectionStringBuilder();

                        connectionStringBuilder.DataSource = (this.ContentGrid.FindName("Server") as TextBox)?.Text ?? string.Empty;
                        connectionStringBuilder.InitialCatalog = (this.ContentGrid.FindName("Database") as TextBox)?.Text ?? string.Empty;
                        connectionStringBuilder.UserID = (this.ContentGrid.FindName("Username") as TextBox)?.Text ?? string.Empty;
                        connectionStringBuilder.Password = (this.ContentGrid.FindName("Password") as TextBox)?.Text ?? string.Empty;
                        connectionStringBuilder.IntegratedSecurity = (this.ContentGrid.FindName("UseWindowsSecurity") as CheckBox)?.IsChecked ?? false;

                        connectionStringBuilder.ApplicationName = "SoluiNet.DevTools";

                        this.Value = connectionStringBuilder.ConnectionString;
                    }
                };

                this.ContentGrid.Children.Add(calculateConnectionStringButton);
                #endregion
            }
            else
            {
                this.AdditionalInfo.Content = "...";
                this.Height = 45;

                var providerNameTextBox = this.ContentGrid.FindName("ProviderName") as UIElement;
                this.ContentGrid.Children.Remove(providerNameTextBox);

                var environmentTextBox = this.ContentGrid.FindName("Environment") as UIElement;
                this.ContentGrid.Children.Remove(environmentTextBox);

                this.Expanded = false;
            }
        }
    }
}
