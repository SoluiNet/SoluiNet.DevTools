// <copyright file="SoluiNetWindow.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.Window
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// The base window class for SoluiNet.DevTools windows.
    /// </summary>
    public class SoluiNetWindow : Window
    {
        /// <summary>
        /// Gets or sets the title format string.
        /// </summary>
        public string TitleFormatString { get; set; }

        /// <summary>
        /// Set the values which can be used for replacing elements in the title.
        /// </summary>
        /// <param name="titleParts">The values which should be inserted to the title.</param>
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
