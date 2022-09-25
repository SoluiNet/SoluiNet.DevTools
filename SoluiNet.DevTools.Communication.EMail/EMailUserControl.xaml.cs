// <copyright file="EMailUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Communication.EMail
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
#if BUILD_FOR_WINDOWS
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
#endif
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.Tools;
    using SoluiNet.DevTools.Core.Tools.Plugin;

    /// <summary>
    /// Interaction logic for EMailUserControl.xaml.
    /// </summary>
    public partial class EMailUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EMailUserControl"/> class.
        /// </summary>
        public EMailUserControl()
        {
            this.InitializeComponent();
        }
    }
}
