﻿// <copyright file="Options.xaml.cs" company="SoluiNet">
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
    using SoluiNet.DevTools.Core.UI.WPF.Window;
    using SoluiNet.DevTools.UI.UserControls;

    /// <summary>
    /// Interaction logic for Options.xaml.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1724:Type names should not match namespaces", Justification = "We want to use keywords for better unterstanding of the purpose.")]
    public partial class Options : SoluiNetWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
            this.InitializeComponent();
        }

        private void ConnectionStringSettings_Selected(object sender, RoutedEventArgs e)
        {
            this.OptionDetails.Children.Clear();

            this.OptionDetails.Children.Add(new ManageConnectionStrings());
        }

        private void Plugins_Selected(object sender, RoutedEventArgs e)
        {
            this.OptionDetails.Children.Clear();

            this.OptionDetails.Children.Add(new PluginConfiguration());
        }

        private void CloseOptions_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
