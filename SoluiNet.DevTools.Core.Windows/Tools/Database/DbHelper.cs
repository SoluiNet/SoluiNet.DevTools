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
    using System.Security.Principal;
    using SoluiNet.DevTools.Core.Tools.Database;
    using SoluiNet.DevTools.Core.Windows.Tools.Security;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Remove unused parameter", Justification = "'impersonation' parameter has been added for future implementations")]
        public static DataTable ExecuteSqlCommand(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsLogin impersonation = null)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Remove unused parameter", Justification = "'impersonation' parameter has been added for future implementations")]
        public static IList<DataTable> ExecuteSqlScript(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsLogin impersonation = null)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1111:Closing parenthesis should be on line of last parameter", Justification = "Couldn't format closing parenthesis correctly with compiler switch.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "Couldn't format closing parenthesis correctly with compiler switch.")]
        public static DataTable ExecuteSqlCommand(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsIdentity impersonationIdentity = null)
        {
            if (impersonationIdentity == null)
            {
                throw new ArgumentNullException(nameof(impersonationIdentity));
            }

            DataTable result = null;

#if COMPILED_FOR_NETSTANDARD
            WindowsIdentity.RunImpersonated(impersonationIdentity.AccessToken, () =>
#else
            using (var context = impersonationIdentity.Impersonate())
#endif
            {
                switch (providerType)
                {
                    case "System.Data.SqlClient":
                        result = ExecuteSqlCommand<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
                        break;
                    case "System.Data.SQLite":
                        result = ExecuteSqlCommand<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
                        break;
                    default:
                        result = null;
                        break;
                }

#if COMPILED_FOR_NETSTANDARD
                return;
#endif
            }
#if COMPILED_FOR_NETSTANDARD
            );
#endif

            return result;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1111:Closing parenthesis should be on line of last parameter", Justification = "Couldn't format closing parenthesis correctly with compiler switch.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "Couldn't format closing parenthesis correctly with compiler switch.")]
        public static IList<DataTable> ExecuteSqlScript(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsIdentity impersonationIdentity = null)
        {
            if (impersonationIdentity == null)
            {
                throw new ArgumentNullException(nameof(impersonationIdentity));
            }

            IList<DataTable> result = null;

#if COMPILED_FOR_NETSTANDARD
            WindowsIdentity.RunImpersonated(impersonationIdentity.AccessToken, () =>
#else
            using (var context = impersonationIdentity.Impersonate())
#endif
            {
                switch (providerType)
                {
                    case "System.Data.SqlClient":
                        result = ExecuteSqlScript<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
                        break;
                    case "System.Data.SQLite":
                        result = ExecuteSqlScript<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
                        break;
                    default:
                        result = null;
                        break;
                }

#if COMPILED_FOR_NETSTANDARD
                return;
#endif
            }
#if COMPILED_FOR_NETSTANDARD
            );
#endif

            return result;
        }
    }
}
