// <copyright file="JsonTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Json
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides a collection of methods to work with JSON elements.
    /// </summary>
    public static class JsonTools
    {
        private const string JsonErrorFormat = @"{{ ""error"": ""{0}"", ""exception"": ""{1}"", ""inner_exception"": ""{2}"" }}";

        /// <summary>
        /// Serialize an object to a JSON string.
        /// </summary>
        /// <param name="serializableObject">The object which should be serialized.</param>
        /// <returns>Returns a string which represents the overgiven object in JSON format.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        public static string Serialize(object serializableObject)
        {
            try
            {
                return JsonConvert.SerializeObject(
                    serializableObject,
                    Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    });
            }
            catch (Exception exception)
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    JsonErrorFormat,
                    "not serializable",
                    exception.Message,
                    exception.InnerException != null ? exception.InnerException.Message : string.Empty);
            }
        }
    }
}
