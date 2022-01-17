namespace SoluiNet.DevTools.UI.Blazor.Application
{
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.UI.Blazor.Application;
    using SoluiNet.DevTools.Core.UI.Blazor.Plugin;
    using SoluiNet.DevTools.Core.UI.UIElement;
    using System.Collections.Generic;

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
    }
}
