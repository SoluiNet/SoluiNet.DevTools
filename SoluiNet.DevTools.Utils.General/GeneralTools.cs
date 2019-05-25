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

        public static long GetDecimalValueForHex(string hexValue)
        {
            hexValue = hexValue.Replace("x", string.Empty);
            long result = 0;
            long.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, null, out result);
            return result;
        }

        public static long GetDecimalValueForHex(char hexValue)
        {
            return GetDecimalValueForHex(hexValue.ToString());
        }
    }
}
