// <copyright file="CategorySettings.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core.UI.WPF.Extensions;
    using SoluiNet.DevTools.Core.UI.WPF.UIElement;
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;

    /// <summary>
    /// Provides an user control to manage category settings.
    /// </summary>
    public partial class CategorySettings : ExtendedConfigurationUserControl
    {
        private readonly Category categoryElement;

        private CheckBox distributeEvenlyTarget = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategorySettings"/> class.
        /// </summary>
        /// <param name="categoryElement">The category element.</param>
        public CategorySettings(Category categoryElement)
            : base(categoryElement)
        {
            this.categoryElement = categoryElement;

            this.Loaded += (sender, eventArgs) =>
            {
                var mainGrid = this.FindChild<Grid>("ExtendedConfigurationGrid");

                if (this.distributeEvenlyTarget == null)
                {
                    this.distributeEvenlyTarget = new CheckBox();

                    this.distributeEvenlyTarget.Height = 25.0;
                    this.distributeEvenlyTarget.Width = 125.0;

                    this.distributeEvenlyTarget.Margin = new Thickness(260, 35, 0, 0);

                    this.distributeEvenlyTarget.Content = "Distribute Evenly Target";

                    mainGrid.Children.Add(this.distributeEvenlyTarget);
                }

                mainGrid.FindChild<Button>("Save").Click += (saveSender, saveEventArgs) =>
                    {
                        this.categoryElement.DistributeEvenlyTarget =
                            this.distributeEvenlyTarget.IsChecked.GetValueOrDefault(false);
                    };
            };
        }
    }
}
