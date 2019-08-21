// <copyright file="AssignmentTargetExtended.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.UI
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
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Core.UI.UIElement;
    using SoluiNet.DevTools.Core.UI.WPF.UIElement;

    /// <summary>
    /// Interaction logic for AssignmentTarget.xaml.
    /// </summary>
    public partial class AssignmentTargetExtended : RemovableUserControl
    {
        /// <summary>
        /// The label.
        /// </summary>
        private string label;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentTargetExtended"/> class.
        /// </summary>
        public AssignmentTargetExtended()
        {
            this.InitializeComponent();

            this.Target.SecondaryActionButton.ToolTip = "Assign a smaller duration";
            this.Target.SecondaryActionButton.Content = "Assign a smaller duration";
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label
        {
            get
            {
                return this.label;
            }

            set
            {
                this.label = value;
                this.Target.PrimaryActionButton.ToolTip = value;
                this.Target.PrimaryActionLabel = value;
            }
        }
    }
}
