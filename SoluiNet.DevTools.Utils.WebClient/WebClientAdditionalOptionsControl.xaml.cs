// <copyright file="WebClientAdditionalOptionsControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.WebClient
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
    /// Interaction logic for WebClientAdditionalOptionsControl.xaml.
    /// </summary>
    public partial class WebClientAdditionalOptionsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientAdditionalOptionsControl"/> class.
        /// </summary>
        public WebClientAdditionalOptionsControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the options that have been set up in this user control.
        /// </summary>
        public Dictionary<string, string> Options
        {
            get
            {
                var options = new Dictionary<string, string>();

                foreach (var option in this.OptionsGrid.Children)
                {
                    if (option is WebClientAdditionalOptionControl control)
                    {
                        options.Add(control.Key.Text, control.Value.Text);
                    }
                }

                return options;
            }
        }

        /// <summary>
        /// Add an option to the user control.
        /// </summary>
        /// <param name="key">The key of the option.</param>
        /// <param name="value">The value of the option.</param>
        public void AddOption(string key, string value)
        {
            this.OptionsGrid.RowDefinitions.Add(new RowDefinition());

            var newOption = new WebClientAdditionalOptionControl();

            newOption.Key.Text = key;
            newOption.Value.Text = value;

            newOption.SetValue(Grid.RowProperty, this.OptionsGrid.RowDefinitions.Count - 1);

            newOption.RemoveElement.Click += (sourceElement, args) =>
            {
                this.OptionsGrid.Children.Remove(newOption);
            };

            this.OptionsGrid.Children.Add(newOption);
        }

        /// <summary>
        /// Event handler for the adding of an additional option.
        /// </summary>
        /// <param name="sender">The triggering UI element.</param>
        /// <param name="e">Arguments used for handling this event.</param>
        private void AddAdditionalOption_Click(object sender, RoutedEventArgs e)
        {
            this.AddOption(string.Empty, string.Empty);
        }
    }
}
