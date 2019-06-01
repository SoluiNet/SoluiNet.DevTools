// <copyright file="IFileEditor.cs" company="SoluiNet">
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
    /// Provides an interface for a plugin that should be usable as file editor.
    /// </summary>
    public interface IFileEditor : IBasePlugin
    {
        /// <summary>
        /// Gets the list of supported file extensions.
        /// </summary>
        List<string> SupportedFileExtensions { get; }

        /// <summary>
        /// Gets the list of supported types.
        /// </summary>
        List<string> SupportedTypes { get; }

        /// <summary>
        /// Open the editor plugin for a string.
        /// </summary>
        /// <param name="content">The string which contains the content for the editor.</param>
        /// <param name="type">The type which should be displayed in the editor.</param>
        void OpenEditor(string content, string type = "");

        /// <summary>
        /// Open the editor plugin for a overgiven file path.
        /// </summary>
        /// <param name="filePath">The file path which should be opened.</param>
        void OpenFileWithEditor(string filePath);

        /// <summary>
        /// Fill the editor menu.
        /// </summary>
        void FillMenu();
    }
}
