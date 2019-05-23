// <copyright file="IFileEditor.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IFileEditor : IBasePlugin
    {
        List<string> SupportedFileExtensions { get; }

        List<string> SupportedTypes { get; }

        void OpenEditor(string content, string type = "");

        void OpenFileWithEditor(string filePath);

        void FillMenu();
    }
}
