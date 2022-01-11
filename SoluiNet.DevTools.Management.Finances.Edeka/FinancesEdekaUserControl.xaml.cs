// <copyright file="FinancesEdekaUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Edeka
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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
    using System.Xml.Linq;
    using SoluiNet.DevTools.Core.Tools.Csv;
    using SoluiNet.DevTools.Core.Tools.File;

    /// <summary>
    /// Interaction logic for FinancesEdekaUserControl.xaml.
    /// </summary>
    public partial class FinancesEdekaUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinancesEdekaUserControl"/> class.
        /// </summary>
        public FinancesEdekaUserControl()
        {
            this.InitializeComponent();
        }
    }
}
