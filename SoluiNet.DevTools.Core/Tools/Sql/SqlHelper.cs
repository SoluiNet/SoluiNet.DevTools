using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Core.Tools.Sql
{
    public static class SqlHelper
    {
        private const string EXECUTE_SCRIPT_PART_KEYWORD = "GO";
        private const string VARIABLE_DECLARATION_KEYWORD = "DECLARE";

        public static bool IsScript(this string sqlCommand)
        {
            if (sqlCommand.Contains(EXECUTE_SCRIPT_PART_KEYWORD))
                return true;

            if (sqlCommand.Contains(VARIABLE_DECLARATION_KEYWORD))
                return true;

            if (sqlCommand.ContainsDdlCommand())
                return true;

            return false;
        }

        public static List<string> GetSingleScripts(this string sqlCommand)
        {
            return sqlCommand.Split(new string[] { EXECUTE_SCRIPT_PART_KEYWORD }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimStart().TrimEnd()).ToList();
        }

        public static bool ContainsDdlCommand(this string sqlCommand)
        {
            var ddlRegex = new Regex("(CREATE|ALTER|DROP) (VIEW|STORED PROCEDURE)", RegexOptions.IgnoreCase);

            if (ddlRegex.IsMatch(sqlCommand))
                return true;

            return false;
        }
    }
}
