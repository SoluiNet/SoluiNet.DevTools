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

namespace SoluiNet.DevTools.UI.UserControls
{
    /// <summary>
    /// Interaktionslogik für ConnectionString.xaml
    /// </summary>
    public partial class ConnectionString : UserControl
    {
        public ConnectionString()
        {
            InitializeComponent();
        }

        private bool Expanded { get; set; }

        public string Value
        {
            get { return ConnectionStringValue.Text; }
            set { ConnectionStringValue.Text = value; }
        }

        public string NameKey
        {
            get { return ConnectionStringName.Text; }
            set { ConnectionStringName.Text = value; }
        }

        public string ProviderName { get; set; }

        public string Environment
        {
            get { return NameKey.Contains(".") ? NameKey.Substring(NameKey.LastIndexOf(".", StringComparison.InvariantCulture) + 1) : "Default"; }
        }

        private void AdditionalInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!this.Expanded)
            {
                this.Expanded = true;

                AdditionalInfo.Content = "^";
                this.Height = 200;

                #region provider name
                var providerNameTextBox = new TextBox()
                {
                    Margin = new Thickness(10, 45, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "ProviderName",
                    Width = 200
                };

                providerNameTextBox.Text = ProviderName;
                providerNameTextBox.TextChanged += (o, args) => { this.ProviderName = ((TextBox)o).Text; };

                ContentGrid.Children.Add(providerNameTextBox);
                #endregion provider name

                #region environment
                var environmentTextBox = new TextBox()
                {
                    Margin = new Thickness(215, 45, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Environment",
                    Width = 200
                };

                environmentTextBox.Text = Environment;

                ContentGrid.Children.Add(environmentTextBox);
                #endregion

                #region server
                var serverTextBox = new TextBox()
                {
                    Margin = new Thickness(10, 70, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Server",
                    Width = 200
                };

                var dataSourceRegex = new Regex("data source=(.+?);", RegexOptions.IgnoreCase);

                var dataSourceMatch = dataSourceRegex.Match(Value);

                if (dataSourceMatch.Success)
                    serverTextBox.Text = dataSourceMatch.Groups[1].Value;

                ContentGrid.Children.Add(serverTextBox);
                #endregion

                #region database
                var databaseTextBox = new TextBox()
                {
                    Margin = new Thickness(215, 70, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Database",
                    Width = 200
                };

                var databaseRegex = new Regex("initial catalog=(.+?);", RegexOptions.IgnoreCase);

                var databaseMatch = databaseRegex.Match(Value);

                if (databaseMatch.Success)
                    databaseTextBox.Text = databaseMatch.Groups[1].Value;

                ContentGrid.Children.Add(databaseTextBox);
                #endregion

                #region username
                var usernameTextBox = new TextBox()
                {
                    Margin = new Thickness(10, 95, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Username",
                    Width = 200
                };

                var usernameRegex = new Regex("user id=(.+?);", RegexOptions.IgnoreCase);

                var usernameMatch = usernameRegex.Match(Value);

                if (usernameMatch.Success)
                    usernameTextBox.Text = usernameMatch.Groups[1].Value;

                ContentGrid.Children.Add(usernameTextBox);
                #endregion

                #region password
                var passwordTextBox = new TextBox()
                {
                    Margin = new Thickness(215, 95, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "Password",
                    Width = 200
                };

                var passwordRegex = new Regex("password=(.+?);", RegexOptions.IgnoreCase);

                var passwordMatch = passwordRegex.Match(Value);

                if (passwordMatch.Success)
                    passwordTextBox.Text = passwordMatch.Groups[1].Value;

                ContentGrid.Children.Add(passwordTextBox);
                #endregion

                #region use windows authentication
                var useWindowsAuthenticationTextBox = new CheckBox()
                {
                    Margin = new Thickness(10, 120, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "UseWindowsSecurity",
                    Width = 200
                };

                var useWindowsAuthenticationRegex = new Regex("integrated security=(.+?);", RegexOptions.IgnoreCase);

                var useWindowsAuthenticationMatch = useWindowsAuthenticationRegex.Match(Value);

                if (useWindowsAuthenticationMatch.Success)
                    useWindowsAuthenticationTextBox.IsChecked = useWindowsAuthenticationMatch.Groups[1].Value == "SSPI" || Convert.ToBoolean(useWindowsAuthenticationMatch.Groups[1].Value);

                ContentGrid.Children.Add(useWindowsAuthenticationTextBox);
                #endregion

                #region calculate connectionstring
                var calculateConnectionStringButton = new Button()
                {
                    Margin = new Thickness(420, 45, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Name = "CalculateConnectionString",
                    Width = 200
                };

                calculateConnectionStringButton.Content = "Calculate ConnectionString";
                calculateConnectionStringButton.Click += (o, args) =>
                {
                    if (ProviderName == "System.Data.SqlClient")
                    {
                        var connectionStringBuilder = new SqlConnectionStringBuilder();

                        connectionStringBuilder.DataSource = (ContentGrid.FindName("Server") as TextBox)?.Text ?? string.Empty;
                        connectionStringBuilder.InitialCatalog = (ContentGrid.FindName("Database") as TextBox)?.Text ?? string.Empty;
                        connectionStringBuilder.UserID = (ContentGrid.FindName("Username") as TextBox)?.Text ?? string.Empty;
                        connectionStringBuilder.Password = (ContentGrid.FindName("Password") as TextBox)?.Text ?? string.Empty;
                        connectionStringBuilder.IntegratedSecurity = (ContentGrid.FindName("UseWindowsSecurity") as CheckBox)?.IsChecked ?? false;

                        connectionStringBuilder.ApplicationName = "SoluiNet.DevTools";

                        Value = connectionStringBuilder.ConnectionString;
                    }
                };

                ContentGrid.Children.Add(calculateConnectionStringButton);
                #endregion
            }
            else
            {
                AdditionalInfo.Content = "...";
                this.Height = 45;

                var providerNameTextBox = ContentGrid.FindName("ProviderName") as UIElement;
                ContentGrid.Children.Remove(providerNameTextBox);

                var environmentTextBox = ContentGrid.FindName("Environment") as UIElement;
                ContentGrid.Children.Remove(environmentTextBox);

                this.Expanded = false;
            }
        }
    }
}
