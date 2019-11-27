// <copyright file="DictionaryHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Dictionary
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a collection of methods to work with dictionaries.
    /// </summary>
    public static class DictionaryHelper
    {
        /// <summary>
        /// Merge two dictionaries.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="firstDictionary">The first dictionary.</param>
        /// <param name="secondDictionary">The second dictionary.</param>
        /// <param name="mergeOperation">The merge operation.</param>
        /// <returns>Returns a merged dictionary.</returns>
        public static Dictionary<TKey, double> Merge<TKey>(
            this Dictionary<TKey, double> firstDictionary,
            Dictionary<TKey, double> secondDictionary,
            string mergeOperation = "sum")
        {
            if (mergeOperation == "sum")
            {
                foreach (var item in secondDictionary)
                {
                    if (firstDictionary.ContainsKey(item.Key))
                    {
                        firstDictionary[item.Key] += item.Value;
                    }
                    else
                    {
                        firstDictionary.Add(item.Key, item.Value);
                    }
                }

                return firstDictionary;
            }

            return null;
        }
    }
}
