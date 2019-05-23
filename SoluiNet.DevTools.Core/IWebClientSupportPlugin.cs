// <copyright file="IWebClientSupportPlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Enums;

    public interface IWebClientSupportPlugin : IBasePlugin
    {
        WebClientFormatEnum Format { get; }

        WebClientTypeEnum Type { get; }
    }
}
