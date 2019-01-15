using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoluiNet.DevTools.Core.Extensions;

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
    }
}
