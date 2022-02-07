// <copyright file="ISoluiNetUiBlazorApp.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI.Blazor.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Core.UI.Application;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;

    /// <summary>
    /// Provides an interface for the SoluiNet.DevTools WPF application.
    /// </summary>
    public interface ISoluiNetUiBlazorApp : ISoluiNetUiApp
    {
        /// <summary>
        /// Gets all available plugins that provide database connectivity functions.
        /// </summary>
        ICollection<ISqlUiPlugin> SqlPlugins { get; }

        /// <summary>
        /// Gets all available plugins that provide smart home functions.
        /// </summary>
        ICollection<ISmartHomeUiPlugin> SmartHomePlugins { get; }

        /// <summary>
        /// Gets all available plugins that provide management functions.
        /// </summary>
        ICollection<IManagementUiPlugin> ManagementPlugins { get; }

        /// <summary>
        /// Gets all available plugins that provide utility functions.
        /// </summary>
        ICollection<IUtilitiesDevPlugin> UtilityPlugins { get; }

        /// <summary>
        /// Gets all assemblies that hold a blazor component.
        /// </summary>
        ICollection<Assembly> Assemblies { get; }

        /// <summary>
        /// Gets all available plugins that provide blazor UI.
        /// </summary>
        ICollection<IBlazorPlugin> BlazorPlugins { get; }

        /// <summary>
        /// Gets a list of all style sheets.
        /// </summary>
        ICollection<string> StyleSheets { get; }

        /// <summary>
        /// Gets a list of all scripts.
        /// </summary>
        ICollection<string> Scripts { get; }

        /// <summary>
        /// Render the style sheets.
        /// </summary>
        /// <returns>Returns a <see cref="string"/> which contains raw HTML to include the style sheets.</returns>
        string RenderCss();

        /// <summary>
        /// Render the scripts.
        /// </summary>
        /// <param name="aboveTheFold">A value indicating whether the scripts should be rendered above the fold or not.</param>
        /// <returns>Returns a <see cref="string"/> which contains raw HTML to include the scripts.</returns>
        string RenderScripts(bool aboveTheFold = false);
    }
}