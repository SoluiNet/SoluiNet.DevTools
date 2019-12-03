// <copyright file="WorkflowExecutionPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.WorkflowExecution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using SoluiNet.DevTools.Core.UI.WPF.Plugin;

    /// <summary>
    /// A plugin which allows to execute workflows.
    /// </summary>
    public class WorkflowExecutionPlugin : IUtilitiesDevPlugin
    {
        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "WebClientPlugin"; }
        }

        /// <summary>
        /// Gets the label for the menu.
        /// </summary>
        public string MenuItemLabel
        {
            get { return "Web Client"; }
        }

        /// <summary>
        /// Call this method if the plugin should be displayed.
        /// </summary>
        /// <param name="displayInPluginContainer">The delegate which should be called for displaying the plugin.</param>
        public void Execute(Action<UserControl> displayInPluginContainer)
        {
            if (displayInPluginContainer == null)
            {
                throw new ArgumentNullException(nameof(displayInPluginContainer));
            }

            displayInPluginContainer(new WorkflowExecutionUserControl());
        }
    }
}
