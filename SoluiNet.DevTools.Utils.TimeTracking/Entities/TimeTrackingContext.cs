﻿namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
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
                var connectionStringSetting = ConfigurationManager.ConnectionStrings[nameOrConnectionString.Replace("name=", string.Empty)];

                if(connectionStringSetting == null) {
                    throw new ArgumentNullException("connectionStringSetting");
                }

                connectionString = connectionStringSetting.ConnectionString;
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

                    appliedVersion = new Version("1.0.0.1");
                }

                if (appliedVersion.CompareTo(new Version("1.0.0.2")) < 0)
                {
                    command.CommandText = "CREATE TABLE Category (CategoryId INTEGER PRIMARY KEY, CategoryName TEXT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Category_UsageTime (CategoryId INTEGER, UsageTimeId INTEGER, Duration INTEGER, PRIMARY KEY (CategoryId, UsageTimeId))";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                    command.Parameters.AddWithValue("$versionNo", "1.0.0.2");
                    command.Parameters.AddWithValue("$appliedAt", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff"));

                    command.ExecuteNonQuery();

                    command.Parameters.Clear();

                    appliedVersion = new Version("1.0.0.2");
                }

                if (appliedVersion.CompareTo(new Version("1.0.0.3")) < 0)
                {
                    command.CommandText = "CREATE TABLE Application (ApplicationId INTEGER PRIMARY KEY, ApplicationName TEXT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "ALTER TABLE UsageTime ADD ApplicationId INTEGER";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                    command.Parameters.AddWithValue("$versionNo", "1.0.0.3");
                    command.Parameters.AddWithValue("$appliedAt", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff"));

                    command.ExecuteNonQuery();

                    command.Parameters.Clear();

                    appliedVersion = new Version("1.0.0.3");
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public virtual DbSet<UsageTime> UsageTime { get; set; }
        public virtual DbSet<VersionHistory> VersionHistory { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryUsageTime> CategoryUsageTime { get; set; }
        public virtual DbSet<Application> Application { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsageTime>()
                .ToTable(typeof(UsageTime).Name)
                .HasKey(x => x.UsageTimeId);

            modelBuilder.Entity<VersionHistory>()
                .ToTable(typeof(VersionHistory).Name)
                .HasKey(x => x.VersionHistoryId);

            modelBuilder.Entity<Category>()
                .ToTable(typeof(Category).Name)
                .HasKey(x => x.CategoryId);

            modelBuilder.Entity<CategoryUsageTime>()
                .ToTable(typeof(CategoryUsageTime).Name)
                .HasKey(x => new { x.CategoryId, x.UsageTimeId });

            modelBuilder.Entity<Application>()
                .ToTable(typeof(Application).Name)
                .HasKey(x => x.ApplicationId);
        }
    }
}