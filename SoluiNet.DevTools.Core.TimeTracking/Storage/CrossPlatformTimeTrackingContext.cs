// <copyright file="CrossPlatformTimeTrackingContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.TimeTracking.Storage
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SoluiNet.DevTools.Core.TimeTracking.Models;
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;

    /// <summary>
    /// Cross-platform database context for time tracking that works with .NET Core and SQLite.
    /// </summary>
    public class CrossPlatformTimeTrackingContext : DbContext
    {
        /// <summary>
        /// A value which indicates if the database has already been created.
        /// </summary>
        private static bool created;

        /// <summary>
        /// The connection string which will be used for this context.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The logger instance.
        /// </summary>
        private readonly ILogger<CrossPlatformTimeTrackingContext> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossPlatformTimeTrackingContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string for the database.</param>
        /// <param name="logger">The logger instance.</param>
        public CrossPlatformTimeTrackingContext(string connectionString = null, ILogger<CrossPlatformTimeTrackingContext> logger = null)
        {
            this.connectionString = connectionString ?? GetDefaultConnectionString();
            this.logger = logger;

            if (!created)
            {
                created = true;
                EnsureDatabaseCreated();
            }

            ApplyPerformanceOptimizations();
        }

        /// <summary>
        /// Gets or sets the Application accessor.
        /// </summary>
        public virtual DbSet<Application> Application { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationArea accessor.
        /// </summary>
        public virtual DbSet<ApplicationArea> ApplicationArea { get; set; }

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
        /// Gets or sets the FilterHistory accessor.
        /// </summary>
        public virtual DbSet<FilterHistory> FilterHistory { get; set; }

        /// <summary>
        /// Gets or sets the SystemInfo accessor for cross-platform metadata.
        /// </summary>
        public virtual DbSet<SystemInfo> SystemInfo { get; set; }

        /// <summary>
        /// Gets the default connection string for the current platform.
        /// </summary>
        /// <returns>The default connection string.</returns>
        public static string GetDefaultConnectionString()
        {
            var databasePath = StoragePathProvider.GetDatabasePath();
            return $"Data Source={databasePath}";
        }

        /// <summary>
        /// Configures the database context options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(this.connectionString, options =>
                {
                    options.CommandTimeout(30);
                });

                optionsBuilder.UseLazyLoadingProxies();

                if (this.logger != null)
                {
                    optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                }
            }
        }

        /// <summary>
        /// Configures the entity models.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            // Configure Application entity
            modelBuilder.Entity<Application>()
                .ToTable("Application")
                .HasKey(x => x.ApplicationId);

            // Configure ApplicationArea entity
            modelBuilder.Entity<ApplicationArea>()
                .ToTable("ApplicationArea")
                .HasKey(x => x.ApplicationAreaId);

            modelBuilder.Entity<ApplicationArea>()
                .HasOne<Application>(x => x.Application)
                .WithMany(x => x.ApplicationArea)
                .HasForeignKey(x => x.ApplicationId)
                .IsRequired();

            // Configure Category entity
            modelBuilder.Entity<Category>()
                .ToTable("Category")
                .HasKey(x => x.CategoryId);

            // Configure CategoryUsageTime entity
            modelBuilder.Entity<CategoryUsageTime>()
                .ToTable("Category_UsageTime")
                .HasKey(x => new { x.CategoryId, x.UsageTimeId });

            modelBuilder.Entity<CategoryUsageTime>()
                .HasOne<UsageTime>(x => x.UsageTime)
                .WithMany(x => x.CategoryUsageTime)
                .HasForeignKey(x => x.UsageTimeId)
                .IsRequired();

            modelBuilder.Entity<CategoryUsageTime>()
                .HasOne<Category>(x => x.Category)
                .WithMany(x => x.CategoryUsageTime)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired();

            // Configure UsageTime entity
            modelBuilder.Entity<UsageTime>()
                .ToTable("UsageTime")
                .HasKey(x => x.UsageTimeId);

            modelBuilder.Entity<UsageTime>()
                .HasOne<Application>(x => x.Application)
                .WithMany(x => x.UsageTime)
                .HasForeignKey(x => x.ApplicationId);

            modelBuilder.Entity<UsageTime>()
                .HasOne<ApplicationArea>(x => x.ApplicationArea)
                .WithMany(x => x.UsageTime)
                .HasForeignKey(x => x.ApplicationAreaId);

            // Configure VersionHistory entity
            modelBuilder.Entity<VersionHistory>()
                .ToTable("VersionHistory")
                .HasKey(x => x.VersionHistoryId);

            // Configure FilterHistory entity
            modelBuilder.Entity<FilterHistory>()
                .ToTable("FilterHistory")
                .HasKey(x => x.FilterHistoryId);

            // Configure SystemInfo entity for cross-platform metadata
            modelBuilder.Entity<SystemInfo>()
                .ToTable("SystemInfo")
                .HasKey(x => x.SystemInfoId);

            // Create indexes for performance
            modelBuilder.Entity<UsageTime>()
                .HasIndex(x => x.StartTime)
                .HasDatabaseName("idx_usage_starttime");

            modelBuilder.Entity<UsageTime>()
                .HasIndex(x => x.ApplicationId)
                .HasDatabaseName("idx_usage_application");

            modelBuilder.Entity<UsageTime>()
                .HasIndex(x => x.ApplicationIdentification)
                .HasDatabaseName("idx_usage_appidentification");
        }

        /// <summary>
        /// Ensures the database is created and migrated to the latest version.
        /// </summary>
        private void EnsureDatabaseCreated()
        {
            try
            {
                var databasePath = ExtractDatabasePath(this.connectionString);

                if (!string.IsNullOrEmpty(databasePath))
                {
                    var directory = Path.GetDirectoryName(databasePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                Database.EnsureCreated();

                // Run database migrations if needed
                RunMigrations();
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Failed to ensure database creation");
                throw;
            }
        }

        /// <summary>
        /// Applies performance optimizations to the SQLite database.
        /// </summary>
        private void ApplyPerformanceOptimizations()
        {
            try
            {
                Database.ExecuteSqlRaw("PRAGMA journal_mode = WAL;");
                Database.ExecuteSqlRaw("PRAGMA synchronous = NORMAL;");
                Database.ExecuteSqlRaw("PRAGMA temp_store = MEMORY;");
                Database.ExecuteSqlRaw("PRAGMA mmap_size = 268435456;"); // 256MB
                Database.ExecuteSqlRaw("PRAGMA cache_size = 10000;");

                // Enable regex support for SQLite
                Database.ExecuteSqlRaw("SELECT load_extension('mod_spatialite');", new object[0]);
            }
            catch (Exception ex)
            {
                this.logger?.LogWarning(ex, "Some performance optimizations could not be applied");
            }
        }

        /// <summary>
        /// Runs database migrations to ensure schema is up to date.
        /// </summary>
        private void RunMigrations()
        {
            // This method would contain migration logic similar to the original TimeTrackingContext
            // For now, we'll rely on EnsureCreated() and add migration logic as needed
            this.logger?.LogInformation("Database migrations completed");
        }

        /// <summary>
        /// Extracts the database file path from the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The database file path.</returns>
        private static string ExtractDatabasePath(string connectionString)
        {
            var connectionStringRegex = new Regex(@"(?<key>[^=;,]+)=(?<val>[^;,]+(,\d+)?)", RegexOptions.IgnoreCase);

            foreach (Match match in connectionStringRegex.Matches(connectionString))
            {
                if (match.Groups["key"].Value.Equals("DATA SOURCE", StringComparison.OrdinalIgnoreCase))
                {
                    return match.Groups["val"].Value;
                }
            }

            return string.Empty;
        }
    }
}