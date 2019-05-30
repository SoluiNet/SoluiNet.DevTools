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
    }
}
