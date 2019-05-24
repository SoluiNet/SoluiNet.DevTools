// <copyright file="StoredProcedure.cs" company="SoluiNet">
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
    /// A class which allows to work with stored procedures of a database.
    /// </summary>
    public class StoredProcedure : IDatabaseElement
    {
        /// <summary>
        /// Gets or sets the stored procedure name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the definition of the stored procedure.
        /// </summary>
        public string BodyDefinition { get; set; }
    }
}
