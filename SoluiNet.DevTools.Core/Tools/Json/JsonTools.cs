// <copyright file="JsonTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides a collection of methods to work with JSON elements.
    /// </summary>
    public static class JsonTools
    {
        /// <summary>
        /// Serialize an object to a JSON string.
        /// </summary>
        /// <param name="serializableObject">The object which should be serialized.</param>
        /// <returns>Returns a string which represents the overgiven object in JSON format.</returns>
        public static string Serialize(object serializableObject)
        {
            return JsonConvert.SerializeObject(serializableObject);
        }
    }
}
