// <copyright file="TwoActionButton.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.UIElement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using SoluiNet.DevTools.Core.Tools.Json;

    /// <summary>
    /// Interaction logic for TwoActionButton.xaml.
    /// </summary>
    public partial class TwoActionButton : UserControl
    {
        private bool hasEnteredUserControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoActionButton"/> class.
        /// </summary>
        public TwoActionButton()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the label of the primary action.
        /// </summary>
        public string PrimaryActionLabel
        {
            get
            {
                return this.PrimaryAction.Content.ToString();
            }

            set
            {
                this.PrimaryAction.Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the label of the secondary action.
        /// </summary>
        public string SecondaryActionLabel
        {
            get
            {
                return this.SecondaryAction.Content.ToString();
            }

            set
            {
                this.SecondaryAction.Content = value;
            }
        }

        /// <summary>
        /// Gets the primary action button.
        /// </summary>
        public Button PrimaryActionButton
        {
            get
            {
                return this.PrimaryAction;
            }
        }

        /// <summary>
        /// Gets the secondary action button.
        /// </summary>
        public Button SecondaryActionButton
        {
            get
            {
                return this.SecondaryAction;
            }
        }

        private void ShowSecondColumn()
        {
            Storyboard storyboard = new Storyboard();

            Duration duration = new Duration(TimeSpan.FromMilliseconds(2000));
            CubicEase ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = ease;
            animation.Duration = duration;
            storyboard.Children.Add(animation);
            animation.From = 0;
            animation.To = this.TwoActionButtonGrid.ActualWidth;
            Storyboard.SetTarget(animation, this.SecondColumn);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ColumnDefinition.MaxWidth)"));

            storyboard.Begin();
        }

        private void HideSecondColumn()
        {
            Storyboard storyboard = new Storyboard();

            Duration duration = new Duration(TimeSpan.FromMilliseconds(2000));
            CubicEase ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            DoubleAnimation animation = new DoubleAnimation();
            animation.EasingFunction = ease;
            animation.Duration = duration;
            storyboard.Children.Add(animation);
            animation.From = this.TwoActionButtonGrid.ActualWidth;
            animation.To = 0;
            Storyboard.SetTarget(animation, this.SecondColumn);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ColumnDefinition.MaxWidth)"));

            storyboard.Begin();
        }

        private void TwoActionButtonGrid_PreviewDragEnter(object sender, DragEventArgs e)
        {
#if DEBUG
            Debug.WriteLine(
                "{2} TwoActionButtonGrid.PreviewDragEnter\r\n  hasEnteredUserControl: {0}\r\n  UserControl: {1}",
                this.hasEnteredUserControl,
                (sender as Grid).Name,
                DateTime.Now);

            if (Mouse.DirectlyOver != null)
            {
                Debug.WriteLine("  Mouse.DirectlyOver: {0}", Mouse.DirectlyOver.ToString());
            }

            var visualHit = VisualTreeHelper.HitTest(sender as Visual, e.GetPosition((UIElement)sender));

            if (visualHit != null)
            {
                Debug.WriteLine("  VisualTreeHelper.HitTest: {0}", visualHit.VisualHit.ToString());
            }
#endif

            if (!this.hasEnteredUserControl)
            {
                this.ShowSecondColumn();
            }

            this.hasEnteredUserControl = true;
        }

        private void TwoActionButtonGrid_PreviewDragLeave(object sender, DragEventArgs e)
        {
#if DEBUG
            Debug.WriteLine(
                "{2} TwoActionButtonGrid.PreviewDragLeave\r\n  hasEnteredUserControl: {0}\r\n  UserControl: {1}",
                this.hasEnteredUserControl,
                (sender as Grid).Name,
                DateTime.Now);

            if (Mouse.DirectlyOver != null)
            {
                Debug.WriteLine("  Mouse.DirectlyOver: {0}", Mouse.DirectlyOver.ToString());
            }

            var visualHit = VisualTreeHelper.HitTest(sender as Visual, e.GetPosition((UIElement)sender));

            if (visualHit != null)
            {
                Debug.WriteLine("  VisualTreeHelper.HitTest: {0}", visualHit.VisualHit.ToString());
            }
#endif

            this.HideSecondColumn();

            this.hasEnteredUserControl = false;
        }
    }
}
