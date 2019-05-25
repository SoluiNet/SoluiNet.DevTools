// <copyright file="ShowText.xaml.cs" company="SoluiNet">
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
    using SoluiNet.DevTools.Core.Formatter;
    using SoluiNet.DevTools.Core.Tools.UI;
    using SoluiNet.DevTools.Core.UI;

    /// <summary>
    /// Interaction logic for ShowText.xaml.
    /// </summary>
    public partial class ShowText : SoluiNetWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowText"/> class.
        /// </summary>
        public ShowText()
        {
            this.InitializeComponent();

            var highlighting = UIHelper.LoadHighlightingDefinition(typeof(ShowText), "SQL.xshd");

            this.TextToShow.SyntaxHighlighting = highlighting;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get { return this.TextToShow.Text; }
            set { this.TextToShow.Text = value; }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FormatSQL_Click(object sender, RoutedEventArgs e)
        {
            var formatter = new SqlFormatter();
            this.TextToShow.Text = formatter.FormatString(this.TextToShow.Text);
        }
    }
}
