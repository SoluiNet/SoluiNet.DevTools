// <copyright file="FreeAtHomePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.SmartHome.BuschJaeger.FreeAtHome
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Security.Principal;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

    /// <summary>
    /// Provides a plugin for the Busch Jaeger free@home system.
    /// </summary>
    public class FreeAtHomePlugin : ISmartHomeUiPlugin
    {
        /// <summary>
        /// Gets the first accent colour.
        /// </summary>
        public Color AccentColour1
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the second accent colour.
        /// </summary>
        public Color AccentColour2
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the foreground colour.
        /// </summary>
        public Color ForegroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the background colour.
        /// </summary>
        public Color BackgroundColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the background accent colour.
        /// </summary>
        public Color BackgroundAccentColour
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        public string EntityName
        {
            get { return "BuschJaeger.FreeAtHome"; }
        }

        /// <summary>
        /// Gets the type definition.
        /// </summary>
        public object TypeDefinition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Display the plugin.
        /// </summary>
        /// <param name="mainGrid">The main grid.</param>
        public void Display(Grid mainGrid)
        {
            throw new NotImplementedException();
        }
    }
}
