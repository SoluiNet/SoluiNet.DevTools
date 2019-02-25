using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoluiNet.DevTools.Core.Extensions;
using SoluiNet.DevTools.Core.Tools.Sql;

namespace SoluiNet.DevTools.Core.Tools.Database
{
    public static class DbHelper
    {
        public static DataTable ExecuteSqlServerCommand(string connectionString, string sqlCommand, string environment = "Default")
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    try
                    {
                        connection.Open();

                        var command = new SqlCommand(sqlCommand, connection);

                        if (sqlCommand.IsSqlQuery())
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                var dataTable = new DataTable("QueryResult");

                                dataTable.Load(reader);

                                return dataTable;
                            }
                        }
                        else
                        {
                            using (var transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    command.Transaction = transaction;

                                    var affectedRows = command.ExecuteNonQuery();

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
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }


        public static List<DataTable> ExecuteSqlServerScript(string connectionString, string sqlCommand, string environment = "Default")
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    try
                    {
                        connection.Open();

                        if (sqlCommand.IsSqlQuery() && !sqlCommand.IsScript())
                        {
                            var command = new SqlCommand(sqlCommand, connection);

                            using (var reader = command.ExecuteReader())
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
                                var command = new SqlCommand(sqlScriptPart, connection);

                                using (var transaction = connection.BeginTransaction())
                                {
                                    try
                                    {
                                        command.Transaction = transaction;

                                        using (var reader = command.ExecuteReader())
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
                                var command = new SqlCommand(sqlScriptPart, connection);

                                using (var transaction = connection.BeginTransaction())
                                {
                                    try
                                    {
                                        command.Transaction = transaction;

                                        var affectedRows = command.ExecuteNonQuery();

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
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
