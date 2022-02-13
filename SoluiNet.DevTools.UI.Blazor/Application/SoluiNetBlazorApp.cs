// <copyright file="SoluiNetBlazorApp.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UI.Blazor.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Plugin;
    using SoluiNet.DevTools.Core.UI.Blazor.Application;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
    using SoluiNet.DevTools.Core.UI.UIElement;

    /// <summary>
    /// The solui.net Blazor App.
    /// </summary>
    public class SoluiNetBlazorApp : BaseSoluiNetApp, ISoluiNetUiBlazorApp
    {
        private List<ISqlUiPlugin> sqlPlugins;
        private List<ISmartHomeUiPlugin> smartHomePlugins;
        private List<IManagementUiPlugin> managementPlugins;
        private List<IUtilitiesDevPlugin> utilityPlugins;
        private List<ISoluiNetUIElement> uiElements;
        private List<IBlazorPlugin> blazorPlugins;
        private List<Assembly> assemblies;
        private List<string?> aboveTheFoldScripts;
        private List<string?> scripts;
        private List<string?> styles;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoluiNetBlazorApp"/> class.
        /// </summary>
        public SoluiNetBlazorApp()
            : base()
        {
            this.assemblies = new List<Assembly>();

            BaseSoluiNetApp.InitializePlugins<IBasePlugin>(this.Plugins);

            this.sqlPlugins = new List<ISqlUiPlugin>();
            BaseSoluiNetApp.InitializePlugins<ISqlUiPlugin>(this.sqlPlugins, this.Plugins);

            this.smartHomePlugins = new List<ISmartHomeUiPlugin>();
            BaseSoluiNetApp.InitializePlugins<ISmartHomeUiPlugin>(this.smartHomePlugins, this.Plugins);

            this.managementPlugins = new List<IManagementUiPlugin>();
            BaseSoluiNetApp.InitializePlugins<IManagementUiPlugin>(this.managementPlugins, this.Plugins);

            this.utilityPlugins = new List<IUtilitiesDevPlugin>();
            BaseSoluiNetApp.InitializePlugins<IUtilitiesDevPlugin>(this.utilityPlugins, this.Plugins);

            this.uiElements = new List<ISoluiNetUIElement>();
            /* BaseSoluiNetApp.InitializePlugins<ISoluiNetUIElement>(this.uiElements, this.Plugins); */

            this.blazorPlugins = new List<IBlazorPlugin>();
            BaseSoluiNetApp.InitializePlugins<IBlazorPlugin>(this.blazorPlugins, this.Plugins, this.assemblies);

            this.aboveTheFoldScripts = new List<string?>();
            this.scripts = new List<string?>();
            this.styles = new List<string?>();

            foreach (var plugin in this.blazorPlugins)
            {
                if (plugin is IHoldsResources resourcesPlugin)
                {
                    this.aboveTheFoldScripts.AddRange(resourcesPlugin.Resources["AboveTheFoldScripts"].Select(x => x.ToString()));
                    this.scripts.AddRange(resourcesPlugin.Resources["Scripts"].Select(x => x.ToString()));
                    this.styles.AddRange(resourcesPlugin.Resources["Styles"].Select(x => x.ToString()));
                }
            }

            this.Initialize();
        }

        /// <inheritdoc/>
        public ICollection<ISqlUiPlugin> SqlPlugins
        {
            get
            {
                return this.sqlPlugins;
            }
        }

        /// <inheritdoc/>
        public ICollection<ISmartHomeUiPlugin> SmartHomePlugins
        {
            get
            {
                return this.smartHomePlugins;
            }
        }

        /// <inheritdoc/>
        public ICollection<IManagementUiPlugin> ManagementPlugins
        {
            get
            {
                return this.managementPlugins;
            }
        }

        /// <inheritdoc/>
        public ICollection<IUtilitiesDevPlugin> UtilityPlugins
        {
            get
            {
                return this.utilityPlugins;
            }
        }

        /// <inheritdoc/>
        public ICollection<ISoluiNetUIElement> UiElements
        {
            get
            {
                return this.uiElements;
            }
        }

        /// <inheritdoc/>
        public ICollection<Assembly> Assemblies
        {
            get
            {
                return this.assemblies;
            }
        }

        /// <inheritdoc/>
        public ICollection<IBlazorPlugin> BlazorPlugins
        {
            get
            {
                return this.blazorPlugins;
            }
        }

        /// <inheritdoc/>
        public ICollection<string> StyleSheets
        {
            get
            {
                return this.styles.Select(x => !string.IsNullOrEmpty(x) ? x : string.Empty).ToList();
            }
        }

        /// <inheritdoc/>
        public ICollection<string> AboveTheFoldScripts
        {
            get
            {
                return this.aboveTheFoldScripts.Select(x => !string.IsNullOrEmpty(x) ? x : string.Empty).ToList();
            }
        }

        /// <inheritdoc/>
        public ICollection<string> Scripts
        {
            get
            {
                return this.scripts.Select(x => !string.IsNullOrEmpty(x) ? x : string.Empty).ToList();
            }
        }

        /// <inheritdoc/>
        public string RenderCss()
        {
            var renderedCode = string.Empty;

            if (this.styles.Count > 0)
            {
                renderedCode += this.StyleSheets.Aggregate((x, y) => string.Format("{0}\r\n{1}", x, y));
            }

            return renderedCode;
        }

        /// <inheritdoc/>
        public string RenderScripts(bool aboveTheFold = false)
        {
            var renderedCode = string.Empty;

            if (aboveTheFold)
            {
                if (this.aboveTheFoldScripts.Count > 0)
                {
                    renderedCode += this.aboveTheFoldScripts.Aggregate((x, y) => string.Format("{0}\r\n{1}", x, y));
                }
            }
            else
            {
                if (this.scripts.Count > 0)
                {
                    renderedCode += this.Scripts.Aggregate((x, y) => string.Format("{0}\r\n{1}", x, y));
                }
            }

            return renderedCode;
        }
    }
}
