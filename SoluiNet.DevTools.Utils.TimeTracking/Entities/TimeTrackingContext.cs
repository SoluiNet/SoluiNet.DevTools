// <copyright file="TimeTrackingContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

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

    /// <summary>
    /// The database context which can be used for time tracking purposes.
    /// </summary>
    public class TimeTrackingContext : DbContext
    {
        /// <summary>
        /// A value which indicates if the database has already been created.
        /// </summary>
        private static bool created = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string for the time tracking database context.</param>
        public TimeTrackingContext(string nameOrConnectionString = "name=TimeTrackingContext")
            : base(new SQLiteConnection() { ConnectionString = GetConnectionString(nameOrConnectionString) }, true)
        {
            if (!created)
            {
                created = true;
                //// Database.CreateIfNotExists();

                CreateIfNotExists();
            }
        }

        /// <summary>
        /// Gets or sets the Application accessor.
        /// </summary>
        public virtual DbSet<Application> Application { get; set; }

        /// <summary>
        /// Gets or sets the Category accessor.
        /// </summary>
        public virtual DbSet<Category> Category { get; set; }

        /// <summary>
        /// Gets or sets the CategoryUsageTime accessor.
        /// </summary>
        public virtual DbSet<CategoryUsageTime> CategoryUsageTime { get; set; }

        /// <summary>
        /// Gets or sets the UsageTime accessor.
        /// </summary>
        public virtual DbSet<UsageTime> UsageTime { get; set; }

        /// <summary>
        /// Gets or sets the VersionHistory accessor.
        /// </summary>
        public virtual DbSet<VersionHistory> VersionHistory { get; set; }

        /// <summary>
        /// Create the database if it doesn't exist.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string for the time tracking database context.</param>
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

                if (appliedVersion.CompareTo(new Version("1.0.0.1")) < 0)
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

                if (appliedVersion.CompareTo(new Version("1.0.0.4")) < 0)
                {
                    command.CommandText = "CREATE TABLE ApplicationArea (ApplicationAreaId INTEGER PRIMARY KEY, ApplicationId INTEGER, AreaName TEXT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "ALTER TABLE UsageTime ADD ApplicationAreaId INTEGER";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                    command.Parameters.AddWithValue("$versionNo", "1.0.0.4");
                    command.Parameters.AddWithValue("$appliedAt", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff"));

                    command.ExecuteNonQuery();

                    command.Parameters.Clear();

                    appliedVersion = new Version("1.0.0.4");
                }

                if (appliedVersion.CompareTo(new Version("1.0.0.5")) < 0)
                {
                    command.CommandText = "ALTER TABLE Category_UsageTime RENAME TO Category_UsageTime_PreV1005";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE Category_UsageTime (CategoryId INTEGER, UsageTimeId INTEGER, Duration DOUBLE, PRIMARY KEY (CategoryId, UsageTimeId))";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Category_UsageTime (CategoryId, UsageTimeId, Duration) " +
                        "SELECT CategoryId, UsageTimeId, Duration FROM Category_UsageTime_PreV1005";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                    command.Parameters.AddWithValue("$versionNo", "1.0.0.5");
                    command.Parameters.AddWithValue("$appliedAt", DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff"));

                    command.ExecuteNonQuery();

                    command.Parameters.Clear();

                    appliedVersion = new Version("1.0.0.5");
                }
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// The event handler for model creation.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>()
                .ToTable(typeof(Application).Name)
                .HasKey(x => x.ApplicationId);

            modelBuilder.Entity<Category>()
                .ToTable(typeof(Category).Name)
                .HasKey(x => x.CategoryId);

            modelBuilder.Entity<CategoryUsageTime>()
                .ToTable("Category_UsageTime")
                .HasKey(x => new { x.CategoryId, x.UsageTimeId });

            modelBuilder.Entity<UsageTime>()
                .ToTable(typeof(UsageTime).Name)
                .HasKey(x => x.UsageTimeId);

            modelBuilder.Entity<VersionHistory>()
                .ToTable(typeof(VersionHistory).Name)
                .HasKey(x => x.VersionHistoryId);
        }

        private static string GetConnectionString(string nameOrConnectionString)
        {
            var connectionString = nameOrConnectionString;

            if (nameOrConnectionString.StartsWith("name="))
            {
                var connectionStringSetting = ConfigurationManager.ConnectionStrings[nameOrConnectionString.Replace("name=", string.Empty)];

                if (connectionStringSetting == null)
                {
                    throw new ArgumentNullException("connectionStringSetting");
                }

                connectionString = connectionStringSetting.ConnectionString;
            }

            return connectionString.ToLower().Replace("%localappdata%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        }
    }
}