// <copyright file="SoluiNetWindow.cs" company="SoluiNet">
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

    public class SoluiNetWindow : Window
    {
        public string TitleFormatString { get; set; }

        public void SetTitleParts(IDictionary<string, string> titleParts)
        {
            if (titleParts == null || !titleParts.Any())
            {
                return;
            }

            this.Title = string.Format(this.TitleFormatString, titleParts.OrderBy(x => x.Key).Select(x => x.Value).ToArray());
        }
    }
}
