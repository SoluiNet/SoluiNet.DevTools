﻿// <copyright file="ExtendedButton.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.UIElement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
#if !COMPILED_FOR_NETSTANDARD
    using System.Windows.Controls;
#endif
    using System.Windows.Input;
#if !COMPILED_FOR_NETSTANDARD
    using System.Windows.Media;
#endif

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
            this.SelectedBackground = new SolidColorBrush(Colors.LightBlue);

            this.OnDependencyValueChanged += this.AdjustWidthForChangedDependencyValue;

            this.PreviewMouseLeftButtonDown += (sender, args) =>
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    this.SwitchSelection();
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1003:Use generic event handler instances",
            Justification = "We want to implement additional functionality in the future. Therefore we want to stay with a custom event handler.")]
        public delegate void DependencyValueChangedHandler(object sender, EventArgs eventArgs);

        /// <summary>
        /// The delegate for the event handler of a element selection.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1003:Use generic event handler instances",
            Justification = "We want to implement additional functionality in the future. Therefore we want to stay with a custom event handler.")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "We prefer the current naming.")]
        public event DependencyValueChangedHandler OnDependencyValueChanged;

        /// <summary>
        /// The element selected event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "We prefer the current naming.")]
        public event ElementSelectedHandler OnElementSelected;

        /// <summary>
        /// Gets a value indicating whether the button width will be set via a relation to dependency value or not.
        /// </summary>
        public bool WidthWithDependency { get; }

        /// <summary>
        /// Gets a value indicating whether the element has been selected or not.
        /// </summary>
        public bool Selected { get; private set; }

        /// <summary>
        /// Gets or sets the background layout for selected elements.
        /// </summary>
        public Brush SelectedBackground { get; set; }

        /// <summary>
        /// Gets or sets the background layout for elements in default state.
        /// </summary>
        public Brush DefaultBackground { get; set; }

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

        /// <summary>
        /// Gets or sets the content of the image. For more information see <see cref="ContentControl" />.
        /// </summary>
        public new object Content
        {
            get
            {
                return base.Content;
            }

            set
            {
                base.Content = value;

                this.Background = this.OnBackgroundColourResolving?.Invoke(value);
                this.DefaultBackground = this.Background;
            }
        }

        /// <summary>
        /// Switch the selection state.
        /// </summary>
        public void SwitchSelection()
        {
            if (!this.Selected)
            {
                this.Selected = true;
                this.Background = this.SelectedBackground;

                this.OnElementSelected?.Invoke(this, new EventArgs());
            }
            else
            {
                this.Selected = false;
                this.Background = this.DefaultBackground;
            }
        }

        /// <summary>
        /// Refresh the button layout.
        /// </summary>
        public void Refresh()
        {
            this.Background = this.OnBackgroundColourResolving?.Invoke(this.Content);
            this.DefaultBackground = this.Background;
        }

        /// <summary>
        /// Render the button.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            this.OnDependencyValueChanged?.Invoke(this, new EventArgs());
        }

        private void AdjustWidthForChangedDependencyValue(object sender, EventArgs eventArgs)
        {
            if (!((sender as ExtendedButton)?.Parent is Grid))
            {
                return;
            }

            if (!(sender as ExtendedButton).WidthWithDependency)
            {
                this.Width = ((sender as ExtendedButton)?.Parent as Grid)?.ActualWidth ?? 100;
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
