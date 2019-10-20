// <copyright file="DbHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Windows.Tools.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SQLite;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.Text;
    using System.Threading.Tasks;
    using SoluiNet.DevTools.Core.Extensions;
    using SoluiNet.DevTools.Core.Tools.Sql;

    /// <summary>
    /// Provides a collection of methods to work with databases.
    /// </summary>
    public static class DbHelper
    {
        /// <summary>
        /// Execute a SQL command on a database which can be connected to with the overgiven provider type. Multiple scripts are possible.
        /// </summary>
        /// <param name="providerType">The provider type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <param name="impersonation">The impersonation which can be used for executing the SQL script.</param>
        /// <returns>Returns a <see cref="List{DataTable}"/> with the results of the SQL command. If provider type isn't supported it returns null.</returns>
        public static List<DataTable> ExecuteSqlScript(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsImpersonationContext impersonation = null)
        {
            if (providerType == "System.Data.SqlClient")
            {
                return Core.Tools.Database.DbHelper.ExecuteSqlScript<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
            }

            if (providerType == "System.Data.SQLite")
            {
                return Core.Tools.Database.DbHelper.ExecuteSqlScript<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
            }

            return null;
        }
    }
}
