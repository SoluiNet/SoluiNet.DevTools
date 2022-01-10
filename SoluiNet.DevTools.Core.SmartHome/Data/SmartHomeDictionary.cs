// <copyright file="SmartHomeDictionary.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.SmartHome.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SoluiNet.DevTools.Core.Data;

    /// <summary>
    /// The smart home dictionary.
    /// </summary>
    public class SmartHomeDictionary : BaseDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmartHomeDictionary"/> class.
        /// </summary>
        /// <param name="dataIdentification">The data identification string.</param>
        /// <exception cref="ArgumentNullException">Throws a <see cref="ArgumentNullException"/> if the <paramref name="dataIdentification"/> is empty.</exception>
        public SmartHomeDictionary(string dataIdentification)
            : base()
        {
            if (string.IsNullOrWhiteSpace(dataIdentification))
            {
                throw new ArgumentNullException(nameof(dataIdentification));
            }

            this.DataIdentification = dataIdentification;
        }

        /// <summary>
        /// Gets an identification for the data which will be stored into this dictionary.
        /// </summary>
        public string DataIdentification { get; }
    }
}
