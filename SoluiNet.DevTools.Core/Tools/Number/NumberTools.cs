// <copyright file="NumberTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Number
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods which allows working with numbers.
    /// </summary>
    public static class NumberTools
    {
        /// <summary>
        /// Get a list of numbers which count from an overgiven start to an overgiven end with a predefined step size.
        /// </summary>
        /// <param name="start">The start number.</param>
        /// <param name="end">The end number.</param>
        /// <param name="stepSize">The step size. If not provided it will default to 10.</param>
        /// <returns>Returns a <see cref="List{T}"/> from start number to end number with the chosen step size.</returns>
        public static IList<int> CountUp(int start, int end, int stepSize = 10)
        {
            var countList = new List<int>();

            for (int i = start; i <= end; i += stepSize)
            {
                countList.Add(i);
            }

            return countList;
        }

        /// <summary>
        /// Get a list of numbers which count from an overgiven start to this variable value with a predefined step size.
        /// </summary>
        /// <param name="end">The end number.</param>
        /// <param name="start">The start number.</param>
        /// <param name="stepSize">The step size. If not provided it will default to 10.</param>
        /// <returns>Returns a <see cref="List{T}"/> from start number to the value of this variable with the chosen step size.</returns>
        public static IList<int> CountFrom(this int end, int start, int stepSize = 10)
        {
            return CountUp(start, end, stepSize);
        }

        /// <summary>
        /// Get the duration in seconds as string.
        /// </summary>
        /// <param name="seconds">The duration in seconds.</param>
        /// <returns>Returns a string that describes the duration in known time segements.</returns>
        public static string ToDurationString(this double seconds)
        {
            var formatString = "{1}m {0}s";

            var remnantSeconds = seconds % 60;
            var minutes = Convert.ToInt32(seconds) / 60;
            var hours = 0;
            var days = 0;
            var weeks = 0;

            if (minutes >= 60)
            {
                hours = minutes / 60;
                minutes = minutes % 60;

                formatString = "{2}h {1}m {0}s";
            }

            if (hours >= 24)
            {
                days = hours / 24;
                hours = hours % 24;

                formatString = "{3}d {2}h {1}m {0}s";
            }

            if (days >= 7)
            {
                weeks = days / 7;
                days = days % 7;

                formatString = "{4}w {3}d {2}h {1}m {0}s";
            }

            return string.Format(formatString, remnantSeconds, minutes, hours, days, weeks);
        }

        /// <summary>
        /// Get the duration in seconds as string.
        /// </summary>
        /// <param name="seconds">The duration in seconds.</param>
        /// <returns>Returns a string that describes the duration in known time segements.</returns>
        public static string ToDurationString(this int seconds)
        {
            var formatString = "{1}m {0}s";

            var remnantSeconds = seconds % 60;
            var minutes = seconds / 60;
            var hours = 0;
            var days = 0;
            var weeks = 0;

            if (minutes >= 60)
            {
                hours = minutes / 60;
                minutes = minutes % 60;

                formatString = "{2}h {1}m {0}s";
            }

            if (hours >= 24)
            {
                days = hours / 24;
                hours = hours % 24;

                formatString = "{3}d {2}h {1}m {0}s";
            }

            if (days >= 7)
            {
                weeks = days / 7;
                days = days % 7;

                formatString = "{4}w {3}d {2}h {1}m {0}s";
            }

            return string.Format(formatString, remnantSeconds, minutes, hours, days, weeks);
        }

        /// <summary>
        /// Get the minute value for the overgiven seconds.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>Returns the minute value for the overgiven seconds.</returns>
        public static double SecondsToMinutes(this double seconds)
        {
            return seconds / 60;
        }

        /// <summary>
        /// Get the hour value for the overgiven seconds.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns>Returns the hour value for the overgiven seconds.</returns>
        public static double SecondsToHours(this double seconds)
        {
            return seconds / 3600;
        }

        /// <summary>
        /// Get the second value for the overgiven minutes.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <returns>Returns the minute value for the overgiven minutes.</returns>
        public static double MinutesToSeconds(this double minutes)
        {
            return minutes * 60;
        }

        /// <summary>
        /// Get the hour value for the overgiven minutes.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <returns>Returns the hour value for the overgiven minutes.</returns>
        public static double MinutesToHours(this double minutes)
        {
            return minutes / 60;
        }

        /// <summary>
        /// Get the second value for the overgiven hours.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <returns>Returns the second value for the overgiven hours.</returns>
        public static double HoursToSeconds(this double hours)
        {
            return hours * 3600;
        }

        /// <summary>
        /// Get the minute value for the overgiven hours.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <returns>Returns the minute value for the overgiven hours.</returns>
        public static double HoursToMinutes(this double hours)
        {
            return hours * 60;
        }

        /// <summary>
        /// Round with a delta.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="delta">The delta.</param>
        /// <returns>Round the number to the nearest multiple of delta.</returns>
        public static double RoundWithDelta(this double value, double delta)
        {
            var minMultiple = Math.Floor(value / delta);
            var maxMultiple = minMultiple + 1;

            var differenceMin = Math.Abs((delta * minMultiple) - value);
            var differenceMax = Math.Abs((delta * maxMultiple) - value);

            if (differenceMin >= differenceMax)
            {
                return delta * maxMultiple;
            }

            return delta * minMultiple;
        }

        /// <summary>
        /// Check if type is a numeric type (Taken from https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number).
        /// </summary>
        /// <param name="instance">An instance of the type.</param>
        /// <returns>Returns true if the instance is a numeric type.</returns>
        public static bool IsNumericType(this object instance)
        {
            return instance.GetType().IsNumericType();
        }

        /// <summary>
        /// Check if type is a numeric type (Taken from https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns true if the type is a numeric type.</returns>
        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
