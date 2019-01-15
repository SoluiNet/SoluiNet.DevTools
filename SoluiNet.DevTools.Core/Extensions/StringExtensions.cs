using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SoluiNet.DevTools.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsSqlQuery(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("select");
        }

        public static bool IsSqlUpdate(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("update");
        }

        public static bool IsSqlInsert(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("insert");
        }

        public static bool IsSqlDelete(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("delete");
        }

        public static bool IsSqlExecute(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("exec");
        }

        public static bool IsSqlDrop(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("drop");
        }

        public static bool IsSqlCreate(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("create");
        }

        public static bool IsSqlAlter(this string sqlCommand)
        {
            return sqlCommand.ToLowerInvariant().TrimStart().StartsWith("alter");
        }
    }
}
