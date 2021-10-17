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
    public partial class CategorySettings
    {
        private readonly Category categoryElement;

        private CheckBox distributeEvenlyTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategorySettings"/> class.
        /// </summary>
        /// <param name="categoryElement">The category element.</param>
        public CategorySettings(Category categoryElement)
        {
            this.InitializeComponent();

            this.categoryElement = categoryElement;

            this.Loaded += (sender, eventArgs) =>
            {
                var extendedConfigurationUserControl = new ExtendedConfigurationUserControl(this.categoryElement);

                this.CategorySettingsGrid.Children.Add(extendedConfigurationUserControl);

                // var mainGrid = extendedConfigurationUserControl.FindChild<Grid>("ExtendedConfigurationGrid");

                if (!(extendedConfigurationUserControl.Content is Grid mainGrid))
                {
                    return;
                }

                if (this.distributeEvenlyTarget == null)
                {
                    this.distributeEvenlyTarget = new CheckBox
                    {
                        Height = 25.0,
                        Width = 200.0,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(260, 35, 0, 0),
                        Content = "Distribute Evenly Target",
                        IsChecked = categoryElement.DistributeEvenlyTarget,
                    };

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
