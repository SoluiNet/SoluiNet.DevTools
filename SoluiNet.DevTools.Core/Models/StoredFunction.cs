﻿// <copyright file="StoredFunction.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StoredFunction : IDatabaseElement
    {
        public string Name { get; set; }

        public string BodyDefinition { get; set; }
    }
}
