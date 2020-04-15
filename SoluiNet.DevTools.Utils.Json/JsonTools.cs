// <copyright file="JsonTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.Json
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CSharp;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Provides a collection of methods for JSON object handling.
    /// </summary>
    public static class JsonTools
    {
        /// <summary>
        /// Formats a JSON string (taken from https://stackoverflow.com/questions/4580397/json-formatter-in-c).
        /// </summary>
        /// <param name="jsonString">The JSON string.</param>
        /// <returns>A formatted JSON string.</returns>
        public static string Format(string jsonString)
        {
            // Example 1
            /* var t = "{\"x\":57,\"y\":57.0,\"z\":\"Yes\"}";
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(t);
            var f = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(f); */

            // Example 2
            /* JToken jt = JToken.Parse(t);
            string formatted = jt.ToString(Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(formatted); */

            // Example 2 in one line
            return JToken.Parse(jsonString).ToString(Formatting.Indented);
        }
    }
}
