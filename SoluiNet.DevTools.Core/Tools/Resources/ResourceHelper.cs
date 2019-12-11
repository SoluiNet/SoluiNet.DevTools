// <copyright file="ResourceHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Resources
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Provides supporting methods to work with resources.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Get resource string based on the calling instance.
        /// </summary>
        /// <param name="classInstance">The calling class instance.</param>
        /// <param name="resourceName">The name of the resource string.</param>
        /// <param name="culture">The culture. Will be set to CurrentCulture if not delivered.</param>
        /// <param name="resourcePath">The resource path. Will be set to "Properties.Resources" if not delivered.</param>
        /// <returns>Returns a resource string for the calling class instance.</returns>
        public static string GetResourceString(this object classInstance, string resourceName, CultureInfo culture = null, string resourcePath = "Properties.Resources")
        {
            if (classInstance == null)
            {
                throw new ArgumentNullException(nameof(classInstance));
            }

            return classInstance.GetType().GetResourceString(resourceName, culture, resourcePath);
        }

        /// <summary>
        /// Get resource string based on the calling class type.
        /// </summary>
        /// <param name="classType">The calling class type.</param>
        /// <param name="resourceName">The name of the resource string.</param>
        /// <param name="culture">The culture. Will be set to CurrentCulture if not delivered.</param>
        /// <param name="resourcePath">The resource path. Will be set to "Properties.Resources" if not delivered.</param>
        /// <returns>Returns a resource string for the calling class type.</returns>
        public static string GetResourceString(this Type classType, string resourceName, CultureInfo culture = null, string resourcePath = "Properties.Resources")
        {
            if (classType == null)
            {
                throw new ArgumentNullException(nameof(classType));
            }

            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }

            var classAssemblyName = classType.Assembly.FullName;

            var baseName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", classAssemblyName, resourcePath);

            var resourceManager = new ResourceManager(baseName, classType.Assembly);

            return resourceManager.GetString(resourceName, culture);
        }
    }
}
