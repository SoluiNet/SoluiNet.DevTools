// <copyright file="SqlSyntaxProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.SyntaxHighlighting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a collection of methods to work with SQL syntaxes.
    /// </summary>
    public static class SqlSyntaxProvider
    {
        private static List<string> tags = new List<string>();
        private static List<char> specials = new List<char>();

        static SqlSyntaxProvider()
        {
            string[] strs =
            {
                "Anchor",
                "Applet",
                "Area",
                "Array",
                "Boolean",
            };

            tags = new List<string>(strs);

            // We also want to know all possible delimiters so adding this stuff.
            char[] chrs =
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
            specials = new List<char>(chrs);
        }

        /// <summary>
        /// Checks if the overgiven tag is a known tag.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        /// <returns>Returns true if the tag is a known tag.</returns>
        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate(string s) { return s.ToLower().Equals(tag.ToLower()); });
        }

        /// <summary>
        /// Get the JS provider.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        /// <returns>Returns true if a known tag contains the overgiven tag.</returns>
        public static List<string> GetJSProvider(string tag)
        {
            return tags.FindAll(delegate(string s) { return s.ToLower().StartsWith(tag.ToLower()); });
        }
    }
}
