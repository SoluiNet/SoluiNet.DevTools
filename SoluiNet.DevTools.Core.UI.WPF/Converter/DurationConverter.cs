// <copyright file="DurationConverter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.WPF.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using SoluiNet.DevTools.Core.Tools.Number;

    /// <summary>
    /// Converts durations to readable strings.
    /// </summary>
    public class DurationConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to a duration string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>Returns a converted duration string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value, CultureInfo.InvariantCulture).ToDurationString();
        }

        /// <summary>
        /// Convert a duration value back to the target type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>Returns a converted duration value in the overgiven target type.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
