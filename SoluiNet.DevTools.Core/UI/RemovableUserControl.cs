// <copyright file="RemovableUserControl.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public class RemovableUserControl : UserControl
    {
        public delegate void ElementRemoval();

        public ElementRemoval RemoveElement { get; set; }
    }
}
