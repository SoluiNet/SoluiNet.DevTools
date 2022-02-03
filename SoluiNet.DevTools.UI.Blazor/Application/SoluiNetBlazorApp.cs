// <copyright file="SoluiNetBlazorApp.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI.Blazor.Application
{
    using System.Collections.Generic;
    using System.Reflection;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.UI.Blazor.Application;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
    using SoluiNet.DevTools.Core.UI.UIElement;

    /// <summary>
    /// The solui.net Blazor App.
    /// </summary>
    public class SoluiNetBlazorApp : BaseSoluiNetApp, ISoluiNetUiBlazorApp
    {
        /// <inheritdoc/>
        public ICollection<ISqlUiPlugin> SqlPlugins
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ICollection<ISmartHomeUiPlugin> SmartHomePlugins
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ICollection<IManagementUiPlugin> ManagementPlugins
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ICollection<IUtilitiesDevPlugin> UtilityPlugins
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ICollection<ISoluiNetUIElement> UiElements
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ICollection<Assembly> Assemblies
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ICollection<IBlazorPlugin> BlazorPlugins
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ICollection<string> StyleSheets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public ICollection<string> Scripts
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public string RenderCss()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string RenderScripts(bool aboveTheFold = false)
        {
            throw new NotImplementedException();
        }
    }
}
