// <copyright file="DbHelper.cs" company="SoluiNet">
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
        /// Execute a SQL command on the database. Only a single script is allowed.
        /// </summary>
        /// <typeparam name="TConnection">The connection type which implements the <see cref="IDbConnection"/> interface.</typeparam>
        /// <typeparam name="TCommand">The command type which implements the <see cref="IDbCommand"/> interface.</typeparam>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="environment">The environment on which the SQL command should be executed. If not provided it will default to "Default".</param>
        /// <returns>Returns a <see cref="DataTable"/> with the results of the SQL command.</returns>
        public static DataTable ExecuteSqlCommand<TConnection, TCommand>(string connectionString, string sqlCommand, string environment = "Default")
            where TConnection : IDbConnection
            where TCommand : IDbCommand
        {
            var connectionType = typeof(TConnection);
            ConstructorInfo connectionConstructor = connectionType.GetConstructor(new[] { typeof(string) });

            var commandType = typeof(TCommand);
            ConstructorInfo commandConstructor = commandType.GetConstructor(new[] { typeof(string), connectionType });

            using (TConnection con = (TConnection)connectionConstructor.Invoke(new object[] { connectionString }))
            {
                try
                {
                    try
                    {
                        con.Open();

                        var cmd = (TCommand)commandConstructor.Invoke(new object[] { sqlCommand, con });

                        if (sqlCommand.IsSqlQuery())
                        {
                            using (var reader = cmd.ExecuteReader())
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
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
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
        public static List<DataTable> ExecuteSqlScript<TConnection, TCommand>(string connectionString, string sqlCommand, string environment = "Default")
           where TConnection : IDbConnection
           where TCommand : IDbCommand
        {
            var connectionType = typeof(TConnection);
            ConstructorInfo connectionConstructor = connectionType.GetConstructor(new[] { typeof(string) });

            var commandType = typeof(TCommand);
            ConstructorInfo commandConstructor = commandType.GetConstructor(new[] { typeof(string), connectionType });

            using (TConnection con = (TConnection)connectionConstructor.Invoke(new object[] { connectionString }))
            {
                try
                {
                    try
                    {
                        con.Open();

                        if (sqlCommand.IsSqlQuery() && !sqlCommand.IsScript())
                        {
                            var cmd = (TCommand)commandConstructor.Invoke(new object[] { sqlCommand, con });

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
                            var cmd = (TCommand)commandConstructor.Invoke(new object[] { sqlCommand, con });

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
                                var cmd = (TCommand)commandConstructor.Invoke(new object[] { sqlCommand, con });

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
                                var cmd = (TCommand)commandConstructor.Invoke(new object[] { sqlCommand, con });

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
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
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
        public static List<DataTable> ExecuteSqlServerScript(string connectionString, string sqlCommand, string environment = "Default")
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
            if (providerType == "System.Data.SqlClient")
            {
                return ExecuteSqlCommand<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
            }

            if (providerType == "System.Data.SQLite")
            {
                return ExecuteSqlCommand<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
            }

            return null;
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
        public static List<DataTable> ExecuteSqlScript(string providerType, string connectionString, string sqlCommand, string environment = "Default", WindowsImpersonationContext impersonation = null)
        {
            if (providerType == "System.Data.SqlClient")
            {
                return ExecuteSqlScript<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
            }

            if (providerType == "System.Data.SQLite")
            {
                return ExecuteSqlScript<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);
            }

            return null;
        }
    }
}
