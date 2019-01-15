using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.SyntaxHighlighting
{
    public static class SqlSyntaxProvider
    {
        static List<string> tags = new List<string>();
        static List<char> specials = new List<char>();

        static SqlSyntaxProvider()
        {
            string[] strs =
            {
                "Anchor",
                "Applet",
                "Area",
                "Array",
                "Boolean"
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
                '\t'
            };
            specials = new List<char>(chrs);
        }
        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate (string s) { return s.ToLower().Equals(tag.ToLower()); });
        }
        public static List<string> GetJSProvider(string tag)
        {
            return tags.FindAll(delegate (string s) { return s.ToLower().StartsWith(tag.ToLower()); });
        }
    }
}
