// <copyright file="Options.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI
{
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
    using System.Windows.Shapes;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.UI.UserControls;

    /// <summary>
    /// Interaktionslogik für Options.xaml
    /// </summary>
    public partial class Options : SoluiNetWindow
    {
        public Options()
        {
            this.InitializeComponent();
        }

        private void ConnectionStringSettings_Selected(object sender, RoutedEventArgs e)
        {
            this.OptionDetails.Children.Clear();

            this.OptionDetails.Children.Add(new ManageConnectionStrings());
        }

        private void CloseOptions_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
