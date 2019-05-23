// <copyright file="IDatabaseElement.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDatabaseElement
    {
        string Name { get; set; }

        string BodyDefinition { get; set; }
    }
}
