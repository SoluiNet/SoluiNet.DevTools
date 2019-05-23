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

    public static class SqlSyntaxProvider
    {
        private static List<string> tags = new List<string>();
        private static List<char> specials = new List<char>();

        public static SqlSyntaxProvider()
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

        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate(string s) { return s.ToLower().Equals(tag.ToLower()); });
        }

        public static List<string> GetJSProvider(string tag)
        {
            return tags.FindAll(delegate(string s) { return s.ToLower().StartsWith(tag.ToLower()); });
        }
    }
}
