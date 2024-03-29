﻿// <copyright file="DbHelper.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Tools.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SQLite;
    using System.Globalization;
    using System.Security.Principal;
    using SoluiNet.DevTools.Core.Extensions;
    using SoluiNet.DevTools.Core.Tools.Sql;

    /// <summary>
    /// Provides a collection of methods to work with databases.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "The DB helper can be inherited in OS specific libraries.")]
    public class DbHelper
    {
        /// <summary>
        /// Execute a SQL command on the database. Only a single script is allowed.
        /// </summary>
        /// <typeparam name="TConnection">The connection type which implements the <see cref="IDbConnection"/> interface.</typeparam>
        /// <typeparam name="TCommand">The command type which implements the <see cref="IDbCommand"/> interface.</typeparam>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <returns>Returns a <see cref="DataTable"/> with the results of the SQL command.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "'environment' parameter has been added for future implementations")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "'environment' parameter has been added for future implementations")]
        public static DataTable ExecuteSqlCommand<TConnection, TCommand>(string connectionString, string sqlCommand, string environment = "Default")
            where TConnection : IDbConnection
            where TCommand : IDbCommand
        {
            var connectionType = typeof(TConnection);
            var connectionConstructor = connectionType.GetConstructor(new[] { typeof(string) });

            var commandType = typeof(TCommand);
            var commandConstructor = commandType.GetConstructor(new[] { typeof(string), connectionType });

            using (var con = (TConnection)connectionConstructor?.Invoke(new object[] { connectionString }))
            {
                try
                {
                    con?.Open();

                    var cmd = (TCommand)commandConstructor?.Invoke(new object[] { sqlCommand, con });

                    if (sqlCommand.IsSqlQuery())
                    {
                        using (var reader = cmd?.ExecuteReader())
                        {
                            var dataTable = new DataTable("QueryResult");

                            dataTable.Load(reader);

                            return dataTable;
                        }
                    }
                    else
                    {
                        using (var transaction = con.BeginTransaction())
                        {
                            try
                            {
                                cmd.Transaction = transaction;

                                var affectedRows = cmd.ExecuteNonQuery();

                                var dataTable = new DataTable("ExecutionResult");

                                dataTable.Columns.Add(new DataColumn("AffectedRows", typeof(int)));

                                dataTable.Rows.Add(affectedRows);

                                transaction.Commit();

                                return dataTable;
                            }
                            catch
                            {
                                transaction.Rollback();

                                throw;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    var dataTable = new DataTable("Error");

                    dataTable.Columns.Add(new DataColumn("ExceptionMessage", typeof(string)));

                    dataTable.Rows.Add(exception.Message + "\r\n" + exception.InnerException);

                    return dataTable;
                }
                finally
                {
                    if (con?.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Execute a SQL command on the database. Multiple scripts are possible.
        /// </summary>
        /// <typeparam name="TConnection">The connection type which implements the <see cref="IDbConnection"/> interface.</typeparam>
        /// <typeparam name="TCommand">The command type which implements the <see cref="IDbCommand"/> interface.</typeparam>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <returns>Returns a <see cref="List{DataTable}"/> with the results of the SQL command.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "All exceptions should be catched and written to log")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "'environment' parameter has been added for future implementations")]
        public static IList<DataTable> ExecuteSqlScript<TConnection, TCommand>(string connectionString, string sqlCommand, string environment = "Default")
           where TConnection : IDbConnection
           where TCommand : IDbCommand
        {
            var connectionType = typeof(TConnection);
            var connectionConstructor = connectionType.GetConstructor(new[] { typeof(string) });

            var commandType = typeof(TCommand);
            var commandConstructor = commandType.GetConstructor(new[] { typeof(string), connectionType });

            using (var con = (TConnection)connectionConstructor?.Invoke(new object[] { connectionString }))
            {
                try
                {
                    con.Open();

                    if (sqlCommand.IsSqlQuery() && !sqlCommand.IsScript())
                    {
                        var cmd = (TCommand)commandConstructor?.Invoke(new object[] { sqlCommand, con });

                        using (var reader = cmd.ExecuteReader())
                        {
                            var dataTable = new DataTable("QueryResult");

                            dataTable.Load(reader);
                            dataTable.ExtendedProperties.Add("SqlCommand", sqlCommand);

                            return new List<DataTable>() { dataTable };
                        }
                    }
                    else if (sqlCommand.IsSqlExecute() && !sqlCommand.IsScript())
                    {
                        var cmd = (TCommand)commandConstructor?.Invoke(new object[] { sqlCommand, con });

                        using (var reader = cmd.ExecuteReader())
                        {
                            var dataTable = new DataTable("QueryResult");

                            dataTable.Load(reader);
                            dataTable.ExtendedProperties.Add("SqlCommand", sqlCommand);

                            return new List<DataTable>() { dataTable };
                        }
                    }
                    else if (sqlCommand.IsScript() && !sqlCommand.ContainsDdlCommand())
                    {
                        var dataTables = new List<DataTable>();

                        foreach (var sqlScriptPart in sqlCommand.GetSingleScripts())
                        {
                            var cmd = (TCommand)commandConstructor?.Invoke(new object[] { sqlCommand, con });

                            using (var transaction = con.BeginTransaction())
                            {
                                try
                                {
                                    cmd.Transaction = transaction;

                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        var dataTable = new DataTable(string.Format(CultureInfo.InvariantCulture, "QueryResult-{0:D}", Guid.NewGuid()));

                                        dataTable.Load(reader);
                                        dataTable.ExtendedProperties.Add("SqlCommand", sqlScriptPart);

                                        transaction.Commit();

                                        dataTables.Add(dataTable);
                                    }
                                }
                                catch
                                {
                                    transaction.Rollback();

                                    throw;
                                }
                            }
                        }

                        return dataTables;
                    }
                    else
                    {
                        var dataTables = new List<DataTable>();

                        foreach (var sqlScriptPart in sqlCommand.GetSingleScripts())
                        {
                            var cmd = (TCommand)commandConstructor?.Invoke(new object[] { sqlCommand, con });

                            using (var transaction = con.BeginTransaction())
                            {
                                try
                                {
                                    cmd.Transaction = transaction;

                                    var affectedRows = cmd.ExecuteNonQuery();

                                    var dataTable = new DataTable(string.Format(CultureInfo.InvariantCulture, "ExecutionResult-{0:D}", Guid.NewGuid()));

                                    dataTable.Columns.Add(new DataColumn("AffectedRows", typeof(int)));
                                    dataTable.ExtendedProperties.Add("SqlCommand", sqlScriptPart);

                                    dataTable.Rows.Add(affectedRows);

                                    transaction.Commit();

                                    dataTables.Add(dataTable);
                                }
                                catch
                                {
                                    transaction.Rollback();

                                    throw;
                                }
                            }
                        }

                        return dataTables;
                    }
                }
                catch (Exception exception)
                {
                    var dataTable = new DataTable("Error");

                    dataTable.Columns.Add(new DataColumn("ExceptionMessage", typeof(string)));

                    dataTable.Rows.Add(exception.Message + "\r\n" + exception.InnerException);

                    return new List<DataTable>() { dataTable };
                }
                finally
                {
                    if (con?.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Execute a SQL command on a SQL Server database. Only a single script is allowed.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <returns>Returns a <see cref="DataTable"/> with the results of the SQL command.</returns>
        public static DataTable ExecuteSqlServerCommand(string connectionString, string sqlCommand, string environment = "Default")
        {
            return ExecuteSqlCommand<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
        }

        /// <summary>
        /// Execute a SQL command on a SQL Server database. Multiple scripts are possible.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <returns>Returns a <see cref="List{DataTable}"/> with the results of the SQL command.</returns>
        public static IList<DataTable> ExecuteSqlServerScript(string connectionString, string sqlCommand, string environment = "Default")
        {
            return ExecuteSqlScript<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
        }

        /// <summary>
        /// Execute a SQL command on a database which can be connected to with the overgiven provider type. Only a single script is allowed.
        /// </summary>
        /// <param name="providerType">The provider type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <returns>Returns a <see cref="DataTable"/> with the results of the SQL command. If provider type isn't supported it returns null.</returns>
        public static DataTable ExecuteSqlCommand(string providerType, string connectionString, string sqlCommand, string environment = "Default")
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
        /// Execute a SQL command on a database which can be connected to with the overgiven provider type. Only a single script is allowed.
        /// </summary>
        /// <param name="providerType">The provider type.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <returns>Returns a <see cref="DataTable"/> with the results of the SQL command. If provider type isn't supported it returns null.</returns>
        public static IList<DataTable> ExecuteSqlScript(string providerType, string connectionString, string sqlCommand, string environment = "Default")
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
