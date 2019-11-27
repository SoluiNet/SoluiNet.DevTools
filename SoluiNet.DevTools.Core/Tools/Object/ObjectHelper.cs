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
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

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
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var instanceType = instance.GetType();

            return instanceType.GetProperties()?.Where(property => !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string) && !typeof(ICollection).IsAssignableFrom(property.PropertyType)).Select(x => x.Name).ToList();
        }
    }
}
