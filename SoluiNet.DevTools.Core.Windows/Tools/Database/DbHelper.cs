// <copyright file="DbHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using SoluiNet.DevTools.Core.Windows.Tools.Security;

namespace SoluiNet.DevTools.Core.Windows.Tools.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SQLite;
    using System.Security.Principal;
    using SoluiNet.DevTools.Core.Tools.Database;

    /// <summary>
    /// Provides a collection of methods to work with databases.
    /// </summary>
    public class DbHelper : Core.Tools.Database.DbHelper
    {
        /// <summary>
        /// Execute a SQL command on a database which can be connected to with the overgiven provider type. Only a single script is allowed.
        /// </summary>
        /// <param name="providerType">The provider type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <param name="impersonation">The impersonation which can be used for executing the SQL command.</param>
        /// <returns>Returns a <see cref="DataTable"/> with the results of the SQL command. If provider type isn't supported it returns null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "'impersonation' parameter has been added for future implementations")]
        public static DataTable ExecuteSqlCommand(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsImpersonationContext impersonation = null)
        {
            switch (providerType)
            {
                case "System.Data.SqlClient":
                    return ExecuteSqlCommand<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
                case "System.Data.SQLite":
                    return ExecuteSqlCommand<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Execute a SQL command on a database which can be connected to with the overgiven provider type. Multiple scripts are possible.
        /// </summary>
        /// <param name="providerType">The provider type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <param name="impersonation">The impersonation which can be used for executing the SQL script.</param>
        /// <returns>Returns a <see cref="List{DataTable}"/> with the results of the SQL command. If provider type isn't supported it returns null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "'impersonation' parameter has been added for future implementations")]
        public static IList<DataTable> ExecuteSqlScript(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsImpersonationContext impersonation = null)
        {
            switch (providerType)
            {
                case "System.Data.SqlClient":
                    return ExecuteSqlScript<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
                case "System.Data.SQLite":
                    return ExecuteSqlScript<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Execute a SQL command on a database which can be connected to with the overgiven provider type. Only a single script is allowed.
        /// </summary>
        /// <param name="providerType">The provider type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <param name="impersonationIdentity">The impersonation identity which can be used for executing the SQL command.</param>
        /// <returns>Returns a <see cref="DataTable"/> with the results of the SQL command. If provider type isn't supported it returns null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "'impersonationIdentity' parameter has been added for future implementations")]
        public static DataTable ExecuteSqlCommand(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsIdentity impersonationIdentity = null)
        {
            if (impersonationIdentity == null)
            {
                throw new ArgumentNullException(nameof(impersonationIdentity));
            }

            using (var context = impersonationIdentity.Impersonate())
            {
                switch (providerType)
                {
                    case "System.Data.SqlClient":
                        return ExecuteSqlCommand<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
                    case "System.Data.SQLite":
                        return ExecuteSqlCommand<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Execute a SQL command on a database which can be connected to with the overgiven provider type. Multiple scripts are possible.
        /// </summary>
        /// <param name="providerType">The provider type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <param name="impersonationIdentity">The impersonation identity which can be used for executing the SQL script.</param>
        /// <returns>Returns a <see cref="List{DataTable}"/> with the results of the SQL command. If provider type isn't supported it returns null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "'impersonationIdentity' parameter has been added for future implementations")]
        public static IList<DataTable> ExecuteSqlScript(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsIdentity impersonationIdentity = null)
        {
            if (impersonationIdentity == null)
            {
                throw new ArgumentNullException(nameof(impersonationIdentity));
            }

            using (var context = impersonationIdentity.Impersonate())
            {
                switch (providerType)
                {
                    case "System.Data.SqlClient":
                        return ExecuteSqlScript<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
                    case "System.Data.SQLite":
                        return ExecuteSqlScript<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
                    default:
                        return null;
                }
            }
        }
    }
}
