// <copyright file="DurationConverter.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.Converter
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
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value).ToDurationString();
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
