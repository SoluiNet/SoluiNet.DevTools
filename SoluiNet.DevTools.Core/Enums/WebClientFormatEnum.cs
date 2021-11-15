// <copyright file="WebClientFormatEnum.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An enumeration for possible web client formats.
    /// </summary>
    [Flags]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "It should be clear that this type is an enum.")]
    public enum WebClientFormatEnum
    {
        /// <summary>
        /// Working on XML-basis
        /// </summary>
        Xml = 1,

        /// <summary>
        /// Working on text-basis
        /// </summary>
        Text = 2,

        /// <summary>
        /// Working on binary-basis
        /// </summary>
        Binary = 4,
    }
}
