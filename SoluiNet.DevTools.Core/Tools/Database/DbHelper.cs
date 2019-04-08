using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SoluiNet.DevTools.Core.Extensions;
using SoluiNet.DevTools.Core.Tools.Sql;

namespace SoluiNet.DevTools.Core.Tools.Database
{
    public static class DbHelper
    {
        public static DataTable ExecuteSqlCommand<connection, command>(string connectionString, string sqlCommand, string environment = "Default")
            where connection : IDbConnection
            where command : IDbCommand
        {
            var connectionType = typeof(connection);
            ConstructorInfo connectionConstructor = connectionType.GetConstructor(new[] { typeof(string) });

            var commandType = typeof(command);
            ConstructorInfo commandConstructor = commandType.GetConstructor(new[] { typeof(string), connectionType });

            using (connection con = (connection)connectionConstructor.Invoke(new object[] { connectionString }))
            {
                try
                {
                    try
                    {
                        con.Open();

                        var cmd = (command)commandConstructor.Invoke(new object[] { sqlCommand, con });

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

        public static List<DataTable> ExecuteSqlScript<connection, command>(string connectionString, string sqlCommand, string environment = "Default")
           where connection : IDbConnection
           where command : IDbCommand
        {
            var connectionType = typeof(connection);
            ConstructorInfo connectionConstructor = connectionType.GetConstructor(new[] { typeof(string) });

            var commandType = typeof(command);
            ConstructorInfo commandConstructor = commandType.GetConstructor(new[] { typeof(string), connectionType });

            using (connection con = (connection)connectionConstructor.Invoke(new object[] { connectionString }))
            {
                try
                {
                    try
                    {
                        con.Open();

                        if (sqlCommand.IsSqlQuery() && !sqlCommand.IsScript())
                        {
                            var cmd = (command)commandConstructor.Invoke(new object[] { sqlCommand, con });

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
                            var cmd = (command)commandConstructor.Invoke(new object[] { sqlCommand, con });

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
                                var cmd = (command)commandConstructor.Invoke(new object[] { sqlCommand, con });

                                using (var transaction = con.BeginTransaction())
                                {
                                    try
                                    {
                                        cmd.Transaction = transaction;

                                        using (var reader = cmd.ExecuteReader())
                                        {
                                            var dataTable = new DataTable(string.Format("QueryResult-{0:D}", Guid.NewGuid()));

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
                                var cmd = (command)commandConstructor.Invoke(new object[] { sqlCommand, con });

                                using (var transaction = con.BeginTransaction())
                                {
                                    try
                                    {
                                        cmd.Transaction = transaction;

                                        var affectedRows = cmd.ExecuteNonQuery();

                                        var dataTable = new DataTable(string.Format("ExecutionResult-{0:D}", Guid.NewGuid()));

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

        public static DataTable ExecuteSqlServerCommand(string connectionString, string sqlCommand, string environment = "Default")
        {
            return ExecuteSqlCommand<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
        }

        public static List<DataTable> ExecuteSqlServerScript(string connectionString, string sqlCommand, string environment = "Default")
        {
            return ExecuteSqlScript<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);
        }

        public static DataTable ExecuteSqlCommand(string providerType, string connectionString, string sqlCommand, string environment = "Default")
        {
            if (providerType == "System.Data.SqlClient")
                return ExecuteSqlCommand<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);

            if (providerType == "System.Data.SQLite")
                return ExecuteSqlCommand<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);

            return null;
        }

        public static List<DataTable> ExecuteSqlScript(string providerType, string connectionString, string sqlCommand, string environment = "Default")
        {
            if (providerType == "System.Data.SqlClient")
                return ExecuteSqlScript<SqlConnection, SqlCommand>(connectionString, sqlCommand, environment);

            if (providerType == "System.Data.SQLite")
                return ExecuteSqlScript<SQLiteConnection, SQLiteCommand>(connectionString, sqlCommand, environment);

            return null;
        }
    }
}
