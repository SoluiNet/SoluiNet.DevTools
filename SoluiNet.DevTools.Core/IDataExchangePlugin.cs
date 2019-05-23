// <copyright file="IDataExchangePlugin.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDataExchangePlugin : IBasePlugin
    {
        List<object> GetData(string whereClause); // use dynamic LINQ?

        List<object> GetData(string entityName, IDictionary<string, object> searchData);

        object SetData(object identifier, IDictionary<string, object> valueData);
    }
}
