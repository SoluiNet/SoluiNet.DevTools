// <copyright file="SqlSyntaxProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.SyntaxHighlighting
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods to work with SQL syntax.
    /// </summary>
    public static class SqlSyntaxProvider
    {
        private static readonly List<char> Specials;
        private static readonly List<string> Tags;

        static SqlSyntaxProvider()
        {
            Specials = new List<char>();
            Tags = new List<string>();

            string[] strings =
            {
                "Anchor",
                "Applet",
                "Area",
                "Array",
                "Boolean",
            };

            Tags = new List<string>(strings);

            // We also want to know all possible delimiters so adding this stuff.
            char[] characters =
            {
                '.',
                ')',
                '(',
                '[',
                ']',
                '>',
                '<',
                ':',
                ';',
                '\n',
                '\t',
            };

            Specials = new List<char>(characters);
        }

        /// <summary>
        /// Checks if the overgiven tag is a known tag.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        /// <returns>Returns true if the tag is a known tag.</returns>
        public static bool IsKnownTag(string tag)
        {
            return Tags.Exists(s =>
                s.ToUpperInvariant().Equals(tag.ToUpperInvariant(), StringComparison.InvariantCulture));
        }

        /// <summary>
        /// Get the JS provider.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        /// <returns>Returns true if a known tag contains the overgiven tag.</returns>
        public static List<string> GetJSProvider(string tag)
        {
            return Tags.FindAll(s =>
                s.ToUpperInvariant().StartsWith(tag.ToUpperInvariant(), StringComparison.InvariantCulture));
        }
    }
}
