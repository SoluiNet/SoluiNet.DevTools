﻿// <copyright file="ExtendedButton.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using System.Windows.Input;
using System.Windows.Media;

namespace SoluiNet.DevTools.Core.UI.UIElement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    /// <summary>
    /// Provides a button width additional features.
    /// </summary>
    public class ExtendedButton : Button
    {
        private double dependencyReferenceValue;
        private double dependencyValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedButton"/> class.
        /// </summary>
        public ExtendedButton()
        {
            this.OnDependencyValueChanged += this.AdjustWidthForChangedDependencyValue;

            this.Click += (sender, args) =>
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    this.OnElementSelected?.Invoke(this, new EventArgs());
                }
            };

            this.Loaded += (sender, args) =>
            {
                this.Background = this.OnBackgroundColourResolving(this.Content);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedButton"/> class.
        /// </summary>
        /// <param name="widthWithDependency">Set to true if button should change its' width based on a reference value.</param>
        public ExtendedButton(bool widthWithDependency)
            : this()
        {
            this.WidthWithDependency = widthWithDependency;
        }

        /// <summary>
        /// The delegate for the event handler of a dependency value change.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public delegate void DependencyValueChangedHandler(object sender, EventArgs eventArgs);

        /// <summary>
        /// The delegate for the event handler of a element selection.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public delegate void ElementSelectedHandler(object sender, EventArgs eventArgs);

        /// <summary>
        /// The delegate for the resolving of the background colour.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <returns>The brush which should be used.</returns>
        public delegate Brush ResolveBackgroundColour(object sender);

        /// <summary>
        /// The dependency value changed event.
        /// </summary>
        public event DependencyValueChangedHandler OnDependencyValueChanged;

        /// <summary>
        /// The element selected event.
        /// </summary>
        public event ElementSelectedHandler OnElementSelected;

        /// <summary>
        /// Gets a value indicating whether the button width will be set via a relation to dependency value or not.
        /// </summary>
        public bool WidthWithDependency { get; }

        /// <summary>
        /// Gets or sets the dependency reference value.
        /// </summary>
        public double DependencyReferenceValue
        {
            get
            {
                return this.dependencyReferenceValue;
            }

            set
            {
                this.dependencyReferenceValue = value;

                this.OnDependencyValueChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets or sets the dependency value.
        /// </summary>
        public double DependencyValue
        {
            get
            {
                return this.dependencyValue;
            }

            set
            {
                this.dependencyValue = value;

                this.OnDependencyValueChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets or sets the background colour resolving event.
        /// </summary>
        public ResolveBackgroundColour OnBackgroundColourResolving { get; set; }

        /// <inheritdoc cref="ContentControl" />
        public new object Content
        {
            get
            {
                return base.Content;

            }

            set
            {
                base.Content = value;

                this.Background = this.OnBackgroundColourResolving(value);
            }
        }

        private void AdjustWidthForChangedDependencyValue(object sender, EventArgs eventArgs)
        {
            if (!((sender as ExtendedButton)?.Parent is Grid))
            {
                return;
            }

            if (!(sender as ExtendedButton).WidthWithDependency)
            {
                return;
            }

            if (Math.Abs((sender as ExtendedButton).DependencyReferenceValue) < 0.0000001)
            {
                return;
            }

            var parentWidth = ((sender as ExtendedButton)?.Parent as Grid)?.ActualWidth;
            var value = (sender as ExtendedButton)?.DependencyValue;
            var totalValue = (sender as ExtendedButton)?.DependencyReferenceValue;

            this.Width = parentWidth.GetValueOrDefault() * (value.GetValueOrDefault() / totalValue.GetValueOrDefault());
        }
    }
}