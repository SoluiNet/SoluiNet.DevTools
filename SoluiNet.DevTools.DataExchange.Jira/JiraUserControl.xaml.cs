using SoluiNet.DevTools.Core;
using SoluiNet.DevTools.Core.Tools;
using System;
using System.Collections.Generic;
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

namespace SoluiNet.DevTools.DataExchange.Jira
{
    /// <summary>
    /// Interaktionslogik für JiraUserControl.xaml
    /// </summary>
    public partial class JiraUserControl : UserControl
    {
        public JiraUserControl()
        {
            InitializeComponent();
        }

        private void ExecuteJql_Click(object sender, RoutedEventArgs e)
        {
            var plugin = PluginHelper.GetPluginByName<IDataExchangePlugin>("JiraDataExchange");

            var result = plugin.GetData("ticket", new Dictionary<string, object> { { "jql", JqlStatement.Text } });

            JiraResultText.Text = result.FirstOrDefault().ToString();
        }
    }
}
