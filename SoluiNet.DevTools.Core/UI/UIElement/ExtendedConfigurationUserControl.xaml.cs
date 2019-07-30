// <copyright file="ExtendedConfigurationUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UIElement
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
    using SoluiNet.DevTools.Core.Tools.XML;
    using SoluiNet.DevTools.Core.UI.XmlData;

    /// <summary>
    /// Interaction logic for ApplicationSettingsUserControl.xaml.
    /// </summary>
    public partial class ExtendedConfigurationUserControl : UserControl
    {
        private IContainsExtendedConfiguration configurableElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedConfigurationUserControl"/> class.
        /// </summary>
        /// <param name="configurableElement">The configurable element.</param>
        public ExtendedConfigurationUserControl(IContainsExtendedConfiguration configurableElement)
        {
            this.InitializeComponent();
            this.configurableElement = configurableElement ?? throw new ArgumentNullException(nameof(configurableElement));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SoluiNetExtendedConfigurationType extendedConfiguration = null;

            if (!string.IsNullOrEmpty(this.configurableElement.ExtendedConfiguration))
            {
                extendedConfiguration = this.configurableElement.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>();
            }

            if (extendedConfiguration == null)
            {
                extendedConfiguration = new SoluiNetExtendedConfigurationType();
            }

            extendedConfiguration.regEx = this.RegEx.Text;
            extendedConfiguration.SoluiNetBrushDefinition = this.BrushDefintion.GetBrushDefinition();

            this.configurableElement.ExtendedConfiguration = extendedConfiguration.SerializeInstance<SoluiNetExtendedConfigurationType>();

            Window.GetWindow(this).Close();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SoluiNetExtendedConfigurationType extendedConfiguration = null;

            if (!string.IsNullOrEmpty(this.configurableElement.ExtendedConfiguration))
            {
                extendedConfiguration = this.configurableElement.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>();
            }

            if (extendedConfiguration == null)
            {
                return;
            }

            this.RegEx.Text = extendedConfiguration.regEx;

            if (extendedConfiguration.SoluiNetBrushDefinition != null && extendedConfiguration.SoluiNetBrushDefinition.typeSpecified)
            {
                this.BrushDefintion.ReadFromBrushDefinition(extendedConfiguration.SoluiNetBrushDefinition);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
