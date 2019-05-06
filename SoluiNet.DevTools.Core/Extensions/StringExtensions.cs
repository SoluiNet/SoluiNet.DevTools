using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SoluiNet.DevTools.Core.Extensions
{
    public static class StringExtensions
    {
        private const string VariableFormat = "<\\[{0}\\]>";

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
        public static string SetEnvironment(this string originalString, string environment = "Default")
        {
            return originalString.Replace("[Environment]", environment);
        }

        public static string InjectSettings(this string originalString, Settings.SoluiNetSettingType settings)
        {
            if (settings == null)
            {
                return originalString;
            }

            var settingsDictionary = new Dictionary<string, string>();

            foreach (var environment in settings.SoluiNetEnvironment)
            {
                foreach (var entry in environment.SoluiNetSettingEntry)
                {
                    settingsDictionary.Add(string.Format("Settings.{0}.{1}", environment.name, entry.name), entry.Value);
                }
            }

            return originalString.Inject(settingsDictionary);
        }

        public static string Inject(this string originalString, Dictionary<string, string> injectionValues)
        {
            var adjustedString = originalString;

            foreach (var injection in injectionValues)
            {
                var replaceRegex = new Regex(string.Format(VariableFormat, injection.Key));

                adjustedString = replaceRegex.Replace(adjustedString, injection.Value);
            }

            return adjustedString;
        }

        public static string InjectCommonValues(this string originalString)
        {
            var commonValueDictionary = new Dictionary<string, string>()
            {
                { "UtcDateTime", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss") },
                { "DateTime", DateTime.Now.ToString("yyyy-MM-dd\"T\"HH:mm:ss") },
                { "UserName", Environment.UserName },
                { "MachineName", Environment.MachineName },
                { "CurrentDirectory", Environment.CurrentDirectory },
                { "UserDomainName", Environment.UserDomainName },
                { "Timestamp", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString() },
                { "UtcTimestamp", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() }
            };

            return originalString.Inject(commonValueDictionary);
        }
    }
}
