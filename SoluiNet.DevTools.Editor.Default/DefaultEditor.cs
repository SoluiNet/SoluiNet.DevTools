// <copyright file="DefaultEditor.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Editor.Default
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core;

    /// <summary>
    /// A plugin which provides an editor for general purposes.
    /// </summary>
    public class DefaultEditor : IFileEditor
    {
        /// <summary>
        /// Gets a list of supported file extensions.
        /// </summary>
        public List<string> SupportedFileExtensions
        {
            get { return new List<string> { Core.Constants.GeneralConstants.AllElementsPlaceholder }; }
        }

        /// <summary>
        /// Gets a list of supported types.
        /// </summary>
        public List<string> SupportedTypes
        {
            get { return new List<string> { Core.Constants.GeneralConstants.AllElementsPlaceholder }; }
        }

        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get { return "DefaultFileEditor"; }
        }

        /// <inheritdoc/>
        public void FillMenu()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OpenEditor(string content, string type = "")
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OpenFileWithEditor(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
