// <copyright file="BrushDefinitionUserControl.xaml.cs" company="SoluiNet">
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
    using SoluiNet.DevTools.Core.Tools.UI;
    using SoluiNet.DevTools.Core.UI.XmlData;

    /// <summary>
    /// Interaction logic for BrushDefinitionUserControl.xaml.
    /// </summary>
    public partial class BrushDefinitionUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrushDefinitionUserControl"/> class.
        /// </summary>
        public BrushDefinitionUserControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Get brush definition for the selected values of this control.
        /// </summary>
        /// <returns>Returns a <see cref="SoluiNetBrushDefinitionType"/> object which represents the selected brush settings.</returns>
        public SoluiNetBrushDefinitionType GetBrushDefinition()
        {
            var brushDefinition = new SoluiNetBrushDefinitionType();

            if (this.BrushDefinitionTypeTabs.SelectedIndex == 0)
            {
                brushDefinition.typeSpecified = true;
                brushDefinition.type = SoluiNetBrushType.SimpleLinearGradient;

                brushDefinition.startColour = this.StartColour.SelectedColor.HasValue ? this.StartColour.SelectedColor.Value.ToHexValue() : "#FFFFFF";
                brushDefinition.endColour = this.EndColour.SelectedColor.HasValue ? this.EndColour.SelectedColor.Value.ToHexValue() : "#FFFFFF";

                brushDefinition.angleSpecified = true;
                brushDefinition.angle = this.Angle.Value.HasValue ? this.Angle.Value.Value : 0.75;
            }
            else if (this.BrushDefinitionTypeTabs.SelectedIndex == 1)
            {
                throw new NotImplementedException();
            }

            return brushDefinition;
        }
    }
}
