// <copyright file="JiraUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.DataExchange.Jira
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
#if BUILD_FOR_WINDOWS
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
#endif
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Plugin;

    /// <summary>
    /// Interaction logic for JiraUserControl.xaml.
    /// </summary>
    public partial class JiraUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JiraUserControl"/> class.
        /// </summary>
        public JiraUserControl()
        {
            this.InitializeComponent();
        }

        private void ExecuteJql_Click(object sender, RoutedEventArgs e)
        {
            var plugin = PluginHelper.GetPluginByName<IAllowsDataExchange>("JiraDataExchange");

            var result = plugin.GetData("ticket", new Dictionary<string, object> { { "jql", this.JqlStatement.Text } });

            this.JiraResultText.Text = result.FirstOrDefault().ToString();
        }
    }
}
