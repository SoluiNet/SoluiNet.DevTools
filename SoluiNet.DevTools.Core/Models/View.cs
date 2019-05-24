// <copyright file="View.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A class which allows to work with views of a database.
    /// </summary>
    public class View : IDatabaseElement
    {
        /// <summary>
        /// Gets or sets the view name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the definition of the view.
        /// </summary>
        public string BodyDefinition { get; set; }
    }
}
