// <copyright file="GeneralTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.General
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods for general purposes.
    /// </summary>
    public class GeneralTools
    {
        /// <summary>
        /// Calculate an array of numbers for the overgiven number and base.
        /// Example:
        /// 13, with base 2 =&gt; (1, 1, 0, 1).
        /// 31, with base 16 =&gt; (1, 15).
        /// </summary>
        /// <param name="originalNumber">The original number.</param>
        /// <param name="baseNumber">The base for which the array should be calculated.</param>
        /// <returns>Returns an array which has been calculated for the original number by the overgiven base.</returns>
        public static long[] GetNumberArrayByBase(long originalNumber, long baseNumber)
        {
            var highestPotence = 0;

            var result = new List<long>();

            for (long i = 0; Math.Pow(baseNumber, i) <= originalNumber; i++)
            {
                highestPotence++;
            }

            for (long i = highestPotence; i >= 0; i--)
            {
                var valence = Convert.ToInt64(Math.Pow(baseNumber, i));

                result.Add(originalNumber / valence);
                originalNumber = originalNumber % valence;
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get the hexadecimal value for an overgiven number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Returns the hexadecimal value as string.</returns>
        public static string GetHexadecimalValue(long number)
        {
            if (number < 10)
            {
                return number.ToString();
            }

            switch (number)
            {
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
            }

            return "0";
        }

        /// <summary>
        /// Get the decimal value for a hex value.
        /// </summary>
        /// <param name="hexValue">The hex value as string.</param>
        /// <returns>Returns the decimal value as long.</returns>
        public static long GetDecimalValueForHex(string hexValue)
        {
            hexValue = hexValue.Replace("x", string.Empty);
            long result = 0;
            long.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out result);
            return result;
        }

        /// <summary>
        /// Get the decimal value for a hex value character.
        /// </summary>
        /// <param name="hexValue">The hex value as character.</param>
        /// <returns>Returns the decimal value as long.</returns>
        public static long GetDecimalValueForHex(char hexValue)
        {
            return GetDecimalValueForHex(hexValue.ToString());
        }
    }
}
