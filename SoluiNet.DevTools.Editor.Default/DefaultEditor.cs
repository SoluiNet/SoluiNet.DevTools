using SoluiNet.DevTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Editor.Default
{
    public class DefaultEditor : IFileEditor
    {
        public List<string> SupportedFileExtensions
        {
            get { return new List<string> { "<ALL>" }; }
        }

        public List<string> SupportedTypes
        {
            get { return new List<string> { "<ALL>" }; }
        }

        public string Name
        {
            get { return "Default File Editor"; }
        }

        public void OpenEditor(string content)
        {
            throw new NotImplementedException();
        }

        public void OpenFileWithEditor(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
