// <copyright file="VsDevEnvToolsUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.VsDevEnv
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
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Tools;

    /// <summary>
    /// Interaction logic for VsDevEnvToolsUserControl.xaml.
    /// </summary>
    public partial class VsDevEnvToolsUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VsDevEnvToolsUserControl"/> class.
        /// </summary>
        public VsDevEnvToolsUserControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event handler for transforming CSharp to UML.
        /// </summary>
        /// <param name="sender">The triggering UI element.</param>
        /// <param name="e">Arguments for handling the event.</param>
        private void TransformCsToUml_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "All files (*.cs)|*.cs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (fileDialog.ShowDialog() == true)
            {
                PluginHelper.GetPluginByName<ITransformPlugin>("TransformUml").Transform(fileDialog.FileName, "xml");
            }
        }
    }
}
