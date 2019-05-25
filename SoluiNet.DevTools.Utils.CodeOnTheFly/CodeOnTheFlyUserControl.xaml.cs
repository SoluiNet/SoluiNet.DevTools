// <copyright file="CodeOnTheFlyUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.CodeOnTheFly
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for CodeOnTheFlyUserControl.xaml.
    /// </summary>
    public partial class CodeOnTheFlyUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeOnTheFlyUserControl"/> class.
        /// </summary>
        public CodeOnTheFlyUserControl()
        {
            this.InitializeComponent();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            this.CodeTabs.SelectedIndex = 1;

            this.Result.Text = CodeOnTheFlyTools.RunDynamicCode(this.Code.Text, this.IsSourceCodeComplete.IsChecked.Value, this.CallingMethod.Text, "CSharp", null);
        }
    }
}
