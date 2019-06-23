// <copyright file="ILinkedPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface which can be executed in other plugins if supported.
    /// </summary>
    public interface ILinkedPlugin
    {
        /// <summary>
        /// Execute a linked function.
        /// </summary>
        /// <param name="callingPlugin">The plugin which calls this method.</param>
        /// <param name="parameters">The parameter list as <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="returnType">The return type.</param>
        /// <returns>Returns the result for a linked function. The type of this result can be found in <paramref name="returnType"/>.</returns>
        object ExecuteLinkedFunction(string callingPlugin, Dictionary<string, object> parameters, out string returnType);
    }
}
