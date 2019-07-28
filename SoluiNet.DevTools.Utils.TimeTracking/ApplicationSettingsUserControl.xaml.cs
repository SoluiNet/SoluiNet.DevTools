// <copyright file="ApplicationSettingsUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking
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
    public partial class ApplicationSettingsUserControl : UserControl
    {
        private Entities.Application application;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsUserControl"/> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public ApplicationSettingsUserControl(Entities.Application application)
        {
            this.InitializeComponent();
            this.application = application ?? throw new ArgumentNullException(nameof(application));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var extendedConfiguration = this.application.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>();

            extendedConfiguration.regEx = this.RegEx.Text;
            extendedConfiguration.SoluiNetBrushDefinition = this.BrushDefintion.GetBrushDefinition();

            this.application.ExtendedConfiguration = extendedConfiguration.SerializeInstance<SoluiNetExtendedConfigurationType>();
        }
    }
}
