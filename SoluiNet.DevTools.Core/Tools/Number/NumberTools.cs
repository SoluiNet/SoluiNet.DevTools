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
    }
}
