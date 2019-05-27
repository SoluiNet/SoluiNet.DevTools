// <copyright file="ICredentialProvider.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface which allows to work with credentials.
    /// </summary>
    public interface ICredentialProvider : IBasePlugin
    {
        /// <summary>
        /// Get credentials for overgiven plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which the credentials should be delivered.</param>
        /// <returns>Returns a <see cref="Dictionary{TKey, TValue}"/> with credentials which have been linked to the overgiven plugin.</returns>
        Dictionary<string, string> GetCredentialsForPlugin(IBasePlugin plugin);

        /// <summary>
        /// Get credentials for overgiven key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns a <see cref="Dictionary{TKey, TValue}"/> with credentials for the overgiven key.</returns>
        Dictionary<string, string> GetCredentialsForKey(string key);
    }
}
