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

    public class DefaultEditor : IFileEditor
    {
        public List<string> SupportedFileExtensions
        {
            get { return new List<string> { Core.Constants.GeneralConstants.AllElementsPlaceholder }; }
        }

        public List<string> SupportedTypes
        {
            get { return new List<string> { Core.Constants.GeneralConstants.AllElementsPlaceholder }; }
        }

        public string Name
        {
            get { return "Default File Editor"; }
        }

        public void FillMenu()
        {
            throw new NotImplementedException();
        }

        public void OpenEditor(string content, string type = "")
        {
            throw new NotImplementedException();
        }

        public void OpenFileWithEditor(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
