// <copyright file="TransformErDiagramPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Transform.ErDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core;
    using SoluiNet.DevTools.Core.Plugin;

    /// <summary>
    /// A plugin which provides transform methods for entity relationship diagrams.
    /// </summary>
    public class TransformErDiagramPlugin : ITransformsData
    {
        /// <inheritdoc/>
        public ICollection<string> SupportedInputFormats
        {
            get
            {
                return new List<string> { "sql", "udl" };
            }
        }

        /// <inheritdoc/>
        public ICollection<string> SupportedOutputFormats
        {
            get
            {
                return new List<string> { "xml", "erd", "jpg" };
            }
        }

        /// <summary>
        /// Gets the technical name of the plugin.
        /// </summary>
        public string Name
        {
            get
            {
                return "TransformErDiagram";
            }
        }

        /// <inheritdoc/>
        public object Transform(string inputFile, string preferedOutputFormat)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object Transform(string inputStream, string inputFormat, string preferedOutputFormat)
        {
            throw new NotImplementedException();
        }
    }
}
