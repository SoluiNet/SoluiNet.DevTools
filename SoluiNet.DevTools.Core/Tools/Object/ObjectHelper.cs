// <copyright file="ObjectHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Object
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Tools.Stream;

    /// <summary>
    /// A collection of methods to work with objects.
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// Get a list of primitive properties for the object.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <returns>Returns a <see cref="List{T}"/> of property names.</returns>
        public static List<string> GetPrimitiveProperties(this object instance)
        {
            var instanceType = instance.GetType();

            return instanceType.GetProperties()?.Where(property => property.PropertyType.IsPrimitive || property.PropertyType == typeof(string)).Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Get a list of object properties for the object.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <returns>Returns a <see cref="List{T}"/> of property names.</returns>
        public static List<string> GetObjectProperties(this object instance)
        {
            var instanceType = instance.GetType();

            return instanceType.GetProperties()?.Where(property => !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string) && !typeof(ICollection).IsAssignableFrom(property.PropertyType)).Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Get a list of embedded resource names for the assembly of the object.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <returns>Returns a <see cref="List{T}"/> of property names.</returns>
        public static List<string> GetEmbeddedResourceNames(this object instance)
        {
            var instanceType = instance.GetType();

            return instanceType.Assembly.GetManifestResourceNames().ToList();
        }

        /// <summary>
        /// Get the content of an embedded resource for the assembly of the object.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="resourceNamespace">The resource namespace.</param>
        /// <returns>Returns the content of the embedded resource.</returns>
        public static string GetEmbeddedResourceContent(this object instance, string resourceName, string resourceNamespace = "")
        {
            var instanceType = instance.GetType();

            var possibleResources = instance.GetEmbeddedResourceNames()
                .Where(x =>
                    x.EndsWith((!string.IsNullOrEmpty(resourceNamespace) ? resourceNamespace + "." : string.Empty) + resourceName));

            if (!possibleResources.Any())
            {
                return null;
            }

            return instanceType.Assembly.GetManifestResourceStream(possibleResources.First()).ReadStringFromStream();
        }
    }
}
