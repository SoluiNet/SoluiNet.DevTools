﻿// <copyright file="TimeTrackingContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    using System;
    using System.Configuration;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.SQLite;
#if BUILD_FOR_WINDOWS
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
#endif
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
#if !BUILD_FOR_WINDOWS
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Proxies;
#endif
#if BUILD_FOR_WINDOWS
    using System.Windows.Media;
#endif
    using NLog;
    using SoluiNet.DevTools.Core.Application;
    using SoluiNet.DevTools.Core.Exceptions;
    using SoluiNet.DevTools.Core.Reference;
    using SoluiNet.DevTools.Core.Tools.XML;
#if BUILD_FOR_WINDOWS
    using SoluiNet.DevTools.Core.UI.WPF.Tools.UI;
#endif
    using SoluiNet.DevTools.Core.XmlData;

    /// <summary>
    /// The database context which can be used for time tracking purposes.
    /// </summary>
#if BUILD_FOR_WINDOWS
    public class TimeTrackingContext : System.Data.Entity.DbContext
#else
    public class TimeTrackingContext : Microsoft.EntityFrameworkCore.DbContext
#endif
    {
        /// <summary>
        /// A value which indicates if the database has already been created.
        /// </summary>
        private static bool created;

        /// <summary>
        /// The connection string which will be used for this context.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// The connection name which will be used for this context.
        /// </summary>
        private string connectionName;

#if BUILD_FOR_WINDOWS
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string for the time tracking database context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The constructor called with parameter contextOwnsConnection=true should activate the disposing of the DbConnection after use.")]
        public TimeTrackingContext(string nameOrConnectionString = "name=TimeTrackingContext")
            : base(new SQLiteConnection() { ConnectionString = GetConnectionString(nameOrConnectionString) }, true)
        {
            if (!created)
            {
                created = true;

                CreateIfNotExists();
            }

            RunPerformanceTweaks();
        }
#endif

#if !BUILD_FOR_WINDOWS
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string for the time tracking database context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The constructor called with parameter contextOwnsConnection=true should activate the disposing of the DbConnection after use.")]
        public TimeTrackingContext(string nameOrConnectionString = "name=TimeTrackingContext")
        {
            this.connectionName = nameOrConnectionString;
            this.connectionString = GetConnectionString(nameOrConnectionString);

            if (!created)
            {
                created = true;

                CreateIfNotExists();
            }

            RunPerformanceTweaks();
        }
#endif

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
        /// Gets the logger.
        /// </summary>
        private static Logger Logger
        {
            get
            {
                return LogManager.GetLogger("timeTracking");
            }
        }

        /// <summary>
        /// Run database clean up.
        /// See also: https://phiresky.github.io/blog/2020/sqlite-performance-tuning/.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string for the time tracking database context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "All exceptions should be catched and written to log")]
        public static void CleanUp(string nameOrConnectionString = "name=TimeTrackingContext")
        {
            if (nameOrConnectionString == null)
            {
                throw new ArgumentNullException(nameof(nameOrConnectionString));
            }

            var connectionString = GetConnectionString(nameOrConnectionString);

#if BUILD_FOR_WINDOWS
            var connection = new SQLiteConnection(connectionString);
#else
            var connection = new EntityConnection(nameOrConnectionString);
#endif

            try
            {
                connection.Open();

#if BUILD_FOR_WINDOWS
                var command = new SQLiteCommand("pragma vacuum;", connection);
#else
                var command = new EntityCommand("pragma vacuum;", connection);
#endif

                try
                {
                    command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Couldn't run database clean up.");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Run database optimization.
        /// See also: https://phiresky.github.io/blog/2020/sqlite-performance-tuning/.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string for the time tracking database context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "All exceptions should be catched and written to log")]
        public static void Optimize(string nameOrConnectionString = "name=TimeTrackingContext")
        {
            if (nameOrConnectionString == null)
            {
                throw new ArgumentNullException(nameof(nameOrConnectionString));
            }

            var connectionString = GetConnectionString(nameOrConnectionString);

#if BUILD_FOR_WINDOWS
            var connection = new SQLiteConnection(connectionString);
#else
            var connection = new EntityConnection(nameOrConnectionString);
#endif

            try
            {
                connection.Open();

#if BUILD_FOR_WINDOWS
                var command = new SQLiteCommand("pragma optimize;", connection);
#else
                var command = new EntityCommand("pragma optimize;", connection);
#endif

                try
                {
                    command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Couldn't run database optimization.");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Run performance tweaking scripts.
        /// See also: https://phiresky.github.io/blog/2020/sqlite-performance-tuning/.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string for the time tracking database context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "All exceptions should be catched and written to log")]
        public static void RunPerformanceTweaks(string nameOrConnectionString = "name=TimeTrackingContext")
        {
            if (nameOrConnectionString == null)
            {
                throw new ArgumentNullException(nameof(nameOrConnectionString));
            }

            var connectionString = GetConnectionString(nameOrConnectionString);

#if BUILD_FOR_WINDOWS
            var connection = new SQLiteConnection(connectionString);
#else
            var connection = new EntityConnection(nameOrConnectionString);
#endif

            try
            {
                connection.Open();

#if BUILD_FOR_WINDOWS
                var command = new SQLiteCommand("pragma journal_mode = WAL;", connection);
#else
                var command = new EntityCommand("pragma journal_mode = WAL;", connection);
#endif

                try
                {
                    command.ExecuteNonQuery();

                    command.CommandText = "pragma synchronous = normal;";
                    command.ExecuteNonQuery();

                    command.CommandText = "pragma temp_store = memory;";
                    command.ExecuteNonQuery();

                    command.CommandText = "pragma mmap_size = 30000000000;";
                    command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Couldn't run database performance tweaks.");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Create the database if it doesn't exist.
        /// </summary>
        /// <param name="nameOrConnectionString">The name or connection string for the time tracking database context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "All exceptions should be catched and written to log")]
        public static void CreateIfNotExists(string nameOrConnectionString = "name=TimeTrackingContext")
        {
            if (nameOrConnectionString == null)
            {
                throw new ArgumentNullException(nameof(nameOrConnectionString));
            }

            var filePath = string.Empty;

            var connectionString = GetConnectionString(nameOrConnectionString);

            var connectionStringRegex = new Regex("(?<key>[^=;,]+)=(?<val>[^;,]+(,\\d+)?)");

            foreach (Match match in connectionStringRegex.Matches(connectionString))
            {
                if (match.Groups["key"].Value.Equals("DATA SOURCE", StringComparison.OrdinalIgnoreCase))
                {
                    filePath = match.Groups["val"].Value;
                }
            }

            if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            {
                SQLiteConnection.CreateFile(filePath);

#if BUILD_FOR_WINDOWS
                var firstConnection = new SQLiteConnection(connectionString);
#else
                var firstConnection = new EntityConnection(nameOrConnectionString);
#endif

                try
                {
                    firstConnection.Open();

#if BUILD_FOR_WINDOWS
                    var createVersionHistory = new SQLiteCommand("CREATE TABLE VersionHistory (VersionHistoryId INTEGER PRIMARY KEY, VersionNumber TEXT, AppliedDateTime TEXT)", firstConnection);
#else
                    var createVersionHistory = new EntityCommand("CREATE TABLE VersionHistory (VersionHistoryId INTEGER PRIMARY KEY, VersionNumber TEXT, AppliedDateTime TEXT)", firstConnection);
#endif

                    try
                    {
                        createVersionHistory.ExecuteNonQuery();

                        createVersionHistory.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        createVersionHistory.Parameters.AddWithValue("$versionNo", "1.0.0.0");
                        createVersionHistory.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        createVersionHistory.ExecuteNonQuery();
                    }
                    finally
                    {
                        createVersionHistory.Dispose();
                    }
                }
                finally
                {
                    firstConnection.Close();
                }
            }

#if BUILD_FOR_WINDOWS
            var connection = new SQLiteConnection(connectionString);
#else
            var connection = new EntityConnection(nameOrConnectionString);
#endif

            try
            {
                connection.Open();

#if BUILD_FOR_WINDOWS
                var command = new SQLiteCommand("SELECT VersionNumber FROM VersionHistory ORDER BY AppliedDateTime DESC LIMIT 1", connection);
#else
                var command = new EntityCommand("SELECT VersionNumber FROM VersionHistory ORDER BY AppliedDateTime DESC LIMIT 1", connection);
#endif

                try
                {
                    var appliedVersion = new Version(command.ExecuteScalar().ToString());

                    if (appliedVersion.CompareTo(new Version("1.0.0.1")) < 0)
                    {
                        command.CommandText =
                            "CREATE TABLE UsageTime (UsageTimeId INTEGER PRIMARY KEY, ApplicationIdentification TEXT, StartTime TEXT, Duration INTEGER)";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.1");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.1");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.2")) < 0)
                    {
                        command.CommandText =
                            "CREATE TABLE Category (CategoryId INTEGER PRIMARY KEY, CategoryName TEXT)";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "CREATE TABLE Category_UsageTime (CategoryId INTEGER, UsageTimeId INTEGER, Duration INTEGER, PRIMARY KEY (CategoryId, UsageTimeId))";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.2");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.2");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.3")) < 0)
                    {
                        command.CommandText =
                            "CREATE TABLE Application (ApplicationId INTEGER PRIMARY KEY, ApplicationName TEXT)";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE UsageTime ADD ApplicationId INTEGER";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.3");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.3");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.4")) < 0)
                    {
                        command.CommandText =
                            "CREATE TABLE ApplicationArea (ApplicationAreaId INTEGER PRIMARY KEY, ApplicationId INTEGER, AreaName TEXT)";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE UsageTime ADD ApplicationAreaId INTEGER";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.4");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.4");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.5")) < 0)
                    {
                        command.CommandText = "ALTER TABLE Category_UsageTime RENAME TO Category_UsageTime_PreV1005";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "CREATE TABLE Category_UsageTime (CategoryId INTEGER, UsageTimeId INTEGER, Duration DOUBLE, PRIMARY KEY (CategoryId, UsageTimeId))";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO Category_UsageTime (CategoryId, UsageTimeId, Duration) " +
                                              "SELECT CategoryId, UsageTimeId, Duration FROM Category_UsageTime_PreV1005";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.5");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.5");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.6")) < 0)
                    {
                        command.CommandText = "ALTER TABLE Application ADD ExtendedConfiguration TEXT";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "UPDATE Application SET ExtendedConfiguration = $extendedConfiguration WHERE ApplicationName = $applicationName";
                        command.Parameters.AddWithValue("$extendedConfiguration", XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("BlueViolet").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromRgb(50, 50, 50).ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            }));
                        command.Parameters.AddWithValue("$applicationName", "Microsoft Visual Studio");

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.6");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.6");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.7")) < 0)
                    {
                        command.CommandText =
                            "UPDATE Application SET ExtendedConfiguration = $extendedConfiguration WHERE ApplicationName = $applicationName";
                        command.Parameters.AddWithValue("$extendedConfiguration", XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DodgerBlue").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("WhiteSmoke").ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            }));
                        command.Parameters.AddWithValue("$applicationName", "Outlook");

                        command.ExecuteNonQuery();

                        command.Parameters["$extendedConfiguration"].Value = XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DarkViolet").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("WhiteSmoke").ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            });
                        command.Parameters["$applicationName"].Value = "Microsoft Teams";
                        command.ExecuteNonQuery();

                        command.Parameters["$extendedConfiguration"].Value = XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("WhiteSmoke").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DeepSkyBlue").ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            });
                        command.Parameters["$applicationName"].Value = "Remote Desktop Manager";
                        command.ExecuteNonQuery();

                        command.Parameters["$extendedConfiguration"].Value = XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DarkGreen").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("WhiteSmoke").ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            });
                        command.Parameters["$applicationName"].Value = "Excel";
                        command.ExecuteNonQuery();

                        command.Parameters["$extendedConfiguration"].Value = XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Lime").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("WhiteSmoke").ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            });
                        command.Parameters["$applicationName"].Value = "Notepad++";
                        command.ExecuteNonQuery();

                        command.Parameters["$extendedConfiguration"].Value = XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("LightBlue").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("WhiteSmoke").ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            });
                        command.Parameters["$applicationName"].Value = "Editor";
                        command.ExecuteNonQuery();

                        command.Parameters["$extendedConfiguration"].Value = XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DarkGray").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("LightBlue").ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            });
                        command.Parameters["$applicationName"].Value = "TortoiseGit";
                        command.ExecuteNonQuery();

                        command.Parameters["$extendedConfiguration"].Value = XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    angle = 0.75,
                                    angleSpecified = true,
                                    startColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DarkBlue").ToHex(),
                                    endColour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("LightBlue").ToHex(),
                                    type = SoluiNetBrushType.SimpleLinearGradient,
                                    typeSpecified = true,
                                },
                            });
                        command.Parameters["$applicationName"].Value = "KeePass";
                        command.ExecuteNonQuery();

                        command.Parameters["$extendedConfiguration"].Value = XmlHelper.Serialize(
                            new SoluiNetExtendedConfigurationType()
                            {
                                SoluiNetBrushDefinition = new SoluiNetBrushDefinitionType()
                                {
                                    type = SoluiNetBrushType.LinearGradient,
                                    typeSpecified = true,
                                    SoluiNetStartPoint = new SoluiNetPointType()
                                    {
                                        xAxis = 0,
                                        xAxisSpecified = true,
                                        yAxis = 0,
                                        yAxisSpecified = true,
                                    },
                                    SoluiNetEndPoint = new SoluiNetPointType()
                                    {
                                        xAxis = 1,
                                        xAxisSpecified = true,
                                        yAxis = 0.2,
                                        yAxisSpecified = true,
                                    },
                                    SoluiNetGradientStop = new SoluiNetGradientStopType[]
                                    {
                                        new SoluiNetGradientStopType()
                                        {
                                            colour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("OrangeRed").ToHex(),
                                            offset = 0.2,
                                        },
                                        new SoluiNetGradientStopType()
                                        {
                                            colour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Yellow").ToHex(),
                                            offset = 0.4,
                                        },
                                        new SoluiNetGradientStopType()
                                        {
                                            colour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("Green").ToHex(),
                                            offset = 0.6,
                                        },
                                        new SoluiNetGradientStopType()
                                        {
                                            colour = ApplicationContext.ResolveSingleton<IColourFactory>("ColourFactory").FromName("DeepSkyBlue").ToHex(),
                                            offset = 0.8,
                                        },
                                    },
                                },
                            });
                        command.Parameters["$applicationName"].Value = "Google Chrome";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.7");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.7");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.8")) < 0)
                    {
                        command.CommandText = "ALTER TABLE UsageTime ADD AdditionalInformation TEXT";
                        command.ExecuteNonQuery();

                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.8");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.8");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.9")) < 0)
                    {
                        command.CommandText = "ALTER TABLE Category ADD ExtendedConfiguration TEXT";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.9");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.9");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.10")) < 0)
                    {
                        command.CommandText = "ALTER TABLE Category ADD DistributeEvenlyTarget BOOLEAN";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.10");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.10");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.11")) < 0)
                    {
                        command.CommandText =
                            "CREATE TABLE FilterHistory (FilterHistoryId INTEGER PRIMARY KEY, FilterString TEXT, LastExecutionDateTime TEXT, ExecutionUser TEXT)";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.11");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.11");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.12")) < 0)
                    {
                        command.CommandText = "ALTER TABLE ApplicationArea ADD ExtendedConfiguration TEXT";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.12");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.12");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.13")) < 0)
                    {
                        command.CommandText = "CREATE INDEX idx_usage_starttime ON UsageTime(StartTime);";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.13");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.13");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.14")) < 0)
                    {
                        command.CommandText = "CREATE INDEX idx_usage_application ON UsageTime(ApplicationId);";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.14");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.14");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.15")) < 0)
                    {
                        command.CommandText = "ALTER TABLE UsageTime ADD ApplicationAutomaticAssigned BOOLEAN;";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE UsageTime ADD CategoryAutomaticAssigned BOOLEAN;";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.15");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.15");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.16")) < 0)
                    {
                        command.CommandText = "ALTER TABLE Category_UsageTime ADD DistributedEvenly BOOLEAN;";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.16");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.16");
                    }

                    if (appliedVersion.CompareTo(new Version("1.0.0.17")) < 0)
                    {
                        command.CommandText = "ALTER TABLE UsageTime ADD ApplicationManualAssigned BOOLEAN;";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE UsageTime ADD CategoryManualAssigned BOOLEAN;";
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                        command.CommandText =
                            "INSERT INTO VersionHistory (VersionNumber, AppliedDateTime) VALUES ($versionNo, $appliedAt)";
                        command.Parameters.AddWithValue("$versionNo", "1.0.0.17");
                        command.Parameters.AddWithValue(
                            "$appliedAt",
                            DateTime.UtcNow.ToString("yyyy-MM-dd\"T\"HH:mm:ss.fff", CultureInfo.InvariantCulture));

                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        appliedVersion = new Version("1.0.0.17");
                    }
                }
                finally
                {
                    command.Dispose();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Couldn't update TimeTracking database.");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(this.connectionString);
            optionsBuilder.UseLazyLoadingProxies();
        }

        /// <summary>
        /// The event handler for model creation.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
#if BUILD_FOR_WINDOWS
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
#else
        protected override void OnModelCreating(ModelBuilder modelBuilder)
#endif
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<Application>()
                .ToTable(typeof(Application).Name)
                .HasKey(x => x.ApplicationId);

            modelBuilder.Entity<ApplicationArea>()
                .ToTable(typeof(ApplicationArea).Name)
                .HasKey(x => x.ApplicationAreaId);

            modelBuilder.Entity<ApplicationArea>()
#if BUILD_FOR_WINDOWS
                .HasRequired<Application>(x => x.Application)
                .WithMany(x => x.ApplicationArea)
                .HasForeignKey(x => x.ApplicationId);
#else
                .HasOne<Application>(x => x.Application)
                .WithMany(x => x.ApplicationArea)
                .HasForeignKey(x => x.ApplicationId)
                .IsRequired();
#endif

            modelBuilder.Entity<Category>()
                .ToTable(typeof(Category).Name)
                .HasKey(x => x.CategoryId);

            modelBuilder.Entity<CategoryUsageTime>()
                .ToTable("Category_UsageTime")
                .HasKey(x => new { x.CategoryId, x.UsageTimeId });

            modelBuilder.Entity<CategoryUsageTime>()
#if BUILD_FOR_WINDOWS
                .HasRequired<UsageTime>(x => x.UsageTime)
                .WithMany(x => x.CategoryUsageTime)
                .HasForeignKey(x => x.UsageTimeId);
#else
                .HasOne<UsageTime>(x => x.UsageTime)
                .WithMany(x => x.CategoryUsageTime)
                .HasForeignKey(x => x.UsageTimeId)
                .IsRequired();
#endif

            modelBuilder.Entity<CategoryUsageTime>()
#if BUILD_FOR_WINDOWS
                .HasRequired<Category>(x => x.Category)
                .WithMany(x => x.CategoryUsageTime)
                .HasForeignKey(x => x.CategoryId);
#else
                .HasOne<Category>(x => x.Category)
                .WithMany(x => x.CategoryUsageTime)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired();
#endif

            modelBuilder.Entity<UsageTime>()
                .ToTable(typeof(UsageTime).Name)
                .HasKey(x => x.UsageTimeId);

            modelBuilder.Entity<UsageTime>()
#if BUILD_FOR_WINDOWS
                .HasOptional<Application>(x => x.Application)
                .WithMany(x => x.UsageTime)
                .HasForeignKey(x => x.ApplicationId);
#else
                .HasOne<Application>(x => x.Application)
                .WithMany(x => x.UsageTime)
                .HasForeignKey(x => x.ApplicationId);
#endif

            modelBuilder.Entity<UsageTime>()
#if BUILD_FOR_WINDOWS
                .HasOptional<ApplicationArea>(x => x.ApplicationArea)
                .WithMany(x => x.UsageTime)
                .HasForeignKey(x => x.ApplicationAreaId);
#else
                .HasOne<ApplicationArea>(x => x.ApplicationArea)
                .WithMany(x => x.UsageTime)
                .HasForeignKey(x => x.ApplicationAreaId);
#endif

            modelBuilder.Entity<VersionHistory>()
                .ToTable(typeof(VersionHistory).Name)
                .HasKey(x => x.VersionHistoryId);

            modelBuilder.Entity<FilterHistory>()
                .ToTable(typeof(FilterHistory).Name)
                .HasKey(x => x.FilterHistoryId);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "We want to know that the setting is empty. The parameter(s) should be fine at this moment.")]
        private static string GetConnectionString(string nameOrConnectionString)
        {
            if (nameOrConnectionString == null)
            {
                throw new ArgumentNullException(nameof(nameOrConnectionString));
            }

            var connectionString = nameOrConnectionString;

            if (nameOrConnectionString.StartsWith("name=", StringComparison.InvariantCulture))
            {
                var connectionStringSetting = ConfigurationManager.ConnectionStrings[nameOrConnectionString.Replace("name=", string.Empty, StringComparison.InvariantCultureIgnoreCase)];

                if (connectionStringSetting == null)
                {
                    throw new SoluiNetException("connectionStringSetting is null");
                }

                connectionString = connectionStringSetting.ConnectionString;
            }

            return connectionString.ToUpperInvariant().Replace("%LOCALAPPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}