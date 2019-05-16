namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class TimeTrackingContext : DbContext
    {
        private static bool _created = false;

        public TimeTrackingContext(string nameOrConnectionString = "name=TimeTrackingContext")
            : base(new SQLiteConnection() { ConnectionString = GetConnectionString(nameOrConnectionString) }, true)
        {
            if (!_created)
            {
                _created = true;
                //Database.CreateIfNotExists();

                CreateIfNotExists();
            }
        }

        private static string GetConnectionString(string nameOrConnectionString)
        {
            var connectionString = nameOrConnectionString;

            if (nameOrConnectionString.StartsWith("name="))
            {
                connectionString = ConfigurationManager.ConnectionStrings[nameOrConnectionString.Replace("name=", string.Empty)].ConnectionString;
            }

            return connectionString.ToLower().Replace("%localappdata%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        }

        public static void CreateIfNotExists(string nameOrConnectionString = "name=TimeTrackingContext")
        {
            var filePath = string.Empty;

            var connectionString = GetConnectionString(nameOrConnectionString);

            var connectionStringRegex = new Regex("(?<key>[^=;,]+)=(?<val>[^;,]+(,\\d+)?)");

            foreach (Match match in connectionStringRegex.Matches(connectionString))
            {
                if (match.Groups["key"].Value.ToLower().Equals("data source"))
                {
                    filePath = match.Groups["val"].Value;
                }
            }

            if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            {
                SQLiteConnection.CreateFile(filePath);

                var firstConnection = new SQLiteConnection(connectionString);

                try
                {
                    firstConnection.Open();

                    var createVersionHistory = new SQLiteCommand("CREATE TABLE VersionHistory (VersionHistoryId INTEGER PRIMARY KEY, VersionNumber TEXT, AppliedDateTime TEXT)", firstConnection);
                    createVersionHistory.ExecuteNonQuery();

                    createVersionHistory.CommandText = "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                    createVersionHistory.Parameters.AddWithValue("$versionNo", "1.0.0.0");
                    createVersionHistory.Parameters.AddWithValue("$appliedAt", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff"));

                    createVersionHistory.ExecuteNonQuery();
                }
                finally
                {
                    firstConnection.Close();
                }
            }

            var connection = new SQLiteConnection(connectionString);

            try
            {
                connection.Open();

                var command = new SQLiteCommand("SELECT VersionNumber FROM VersionHistory ORDER BY AppliedDateTime DESC LIMIT 1", connection);

                var appliedVersion = new Version(command.ExecuteScalar().ToString());

                if(appliedVersion.CompareTo(new Version("1.0.0.1")) < 0)
                {
                    command.CommandText = "CREATE TABLE UsageTime (UsageTimeId INTEGER PRIMARY KEY, ApplicationIdentification TEXT, StartTime TEXT, Duration INTEGER)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                    command.Parameters.AddWithValue("$versionNo", "1.0.0.1");
                    command.Parameters.AddWithValue("$appliedAt", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff"));

                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public virtual DbSet<UsageTime> UsageTime { get; set; }
        public virtual DbSet<VersionHistory> VersionHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsageTime>()
                .ToTable(typeof(UsageTime).Name)
                .HasKey(x => x.UsageTimeId);

            modelBuilder.Entity<VersionHistory>()
                .ToTable(typeof(VersionHistory).Name)
                .HasKey(x => x.VersionHistoryId);
        }
    }
}