// <copyright file="GeneralConstants.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Constants
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides general constants.
    /// </summary>
    public static class GeneralConstants
    {
        /// <summary>
        /// Gets the placeholder for all elements.
        /// </summary>
        public static string AllElementsPlaceholder
        {
            get { return "<ALL>"; }
        }

        /// <summary>
        /// Gets the search pattern for duration strings.
        /// </summary>
        public static string DurationSearchPattern
        {
            get { return "(\\s+\\((\\d+w)?\\s*(\\d+d)?\\s*(\\d+h)?\\s*(\\d+m)?\\s*(\\d+s)?\\))"; }
        }
    }
}
