// <copyright file="PlaceholderTextBox.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.UIElement
{
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Provides a text box which will allow to set place holder texts.
    /// </summary>
    public class PlaceholderTextBox : TextBox
    {
        private string placeholderText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceholderTextBox"/> class.
        /// </summary>
        public PlaceholderTextBox()
        {
            this.GotFocus += (sender, eventArgs) =>
            {
                if (base.Text == this.PlaceholderText)
                {
                    this.Text = string.Empty;
                }
            };

            this.LostFocus += (sender, eventArgs) =>
            {
                if (string.IsNullOrWhiteSpace(base.Text))
                {
                    this.Text = this.PlaceholderText;
                }
            };

            this.PlaceholderTextChanged += (sender, eventArgs) =>
            {
                if (base.Text == eventArgs.OldPlaceholderText)
                {
                    this.Text = eventArgs.NewPlaceholderText;
                }
            };

            this.TextChanged += (sender, eventArgs) =>
            {
                this.Foreground = base.Text == this.PlaceholderText ? new SolidColorBrush(Colors.DimGray) : new SolidColorBrush(Colors.Black);
            };
        }

        /// <summary>
        /// The event handler type which will be called if the placeholder text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public delegate void PlaceholderTextChangedEventHandler(object sender, PlaceholderTextChangedEventArgs eventArgs);

        /// <summary>
        /// If the placeholder text has been changed this event will be called.
        /// </summary>
        public event PlaceholderTextChangedEventHandler PlaceholderTextChanged;

        /// <summary>
        /// Gets or sets the placeholder text.
        /// </summary>
        public string PlaceholderText
        {
            get
            {
                return this.placeholderText;
            }

            set
            {
                var oldValue = this.placeholderText;

                this.placeholderText = value;
                this.PlaceholderTextChanged?.Invoke(this, new PlaceholderTextChangedEventArgs(oldValue, value));
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public new string Text
        {
            get
            {
                return (base.Text == this.PlaceholderText) ? string.Empty : base.Text;
            }

            set
            {
                base.Text = value;
            }
        }
    }
}
