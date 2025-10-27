// <copyright file="ConfigurationMigrationTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Configuration;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Tests for configuration migration functionality.
    /// </summary>
    [TestClass]
    public class ConfigurationMigrationTest
    {
        private IPlatformService platformService;
        private CrossPlatformConfigurationManager configurationManager;
        private ConfigurationMigrationService migrationService;
        private string testDirectory;

        /// <summary>
        /// Initialize test setup.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.platformService = PlatformServiceFactory.Create();
            this.configurationManager = new CrossPlatformConfigurationManager(this.platformService);
            this.migrationService = new ConfigurationMigrationService(this.configurationManager, this.platformService);
            
            // Create a temporary test directory
            this.testDirectory = Path.Combine(Path.GetTempPath(), "SoluiNetMigrationTest_" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(this.testDirectory);
        }

        /// <summary>
        /// Clean up test resources.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(this.testDirectory))
            {
                try
                {
                    Directory.Delete(this.testDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        /// <summary>
        /// Test migration service creation with null parameters.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MigrationService_Constructor_ShouldThrowOnNullConfigurationManager()
        {
            // Act
            _ = new ConfigurationMigrationService(null, this.platformService);
        }

        /// <summary>
        /// Test migration service creation with null platform service.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MigrationService_Constructor_ShouldThrowOnNullPlatformService()
        {
            // Act
            _ = new ConfigurationMigrationService(this.configurationManager, null);
        }

        /// <summary>
        /// Test migration from Windows locations when no files exist.
        /// </summary>
        [TestMethod]
        public void MigrateFromWindowsLocations_ShouldReturnFalseWhenNoFilesExist()
        {
            // Act
            var result = this.migrationService.MigrateFromWindowsLocations();

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test individual configuration migration with valid files.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldSucceedWithValidFile()
        {
            // Arrange
            var oldConfigPath = Path.Combine(this.testDirectory, "old-settings.json");
            var configContent = "{\"Version\":\"1.0.0\",\"EnabledPlugins\":{\"TestPlugin\":true}}";
            File.WriteAllText(oldConfigPath, configContent);

            var configName = "migrated-settings";

            // Act
            var result = this.migrationService.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(this.configurationManager.ConfigurationExists(configName));
        }

        /// <summary>
        /// Test individual configuration migration with non-existent file.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldReturnFalseForNonExistentFile()
        {
            // Arrange
            var oldConfigPath = Path.Combine(this.testDirectory, "non-existent.json");
            var configName = "test-config";

            // Act
            var result = this.migrationService.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test migration with null or empty parameters.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldReturnFalseForInvalidParameters()
        {
            // Act & Assert
            Assert.IsFalse(this.migrationService.MigrateConfiguration(null, "test"));
            Assert.IsFalse(this.migrationService.MigrateConfiguration("test.json", null));
            Assert.IsFalse(this.migrationService.MigrateConfiguration(string.Empty, "test"));
            Assert.IsFalse(this.migrationService.MigrateConfiguration("test.json", string.Empty));
        }

        /// <summary>
        /// Test that migration doesn't overwrite existing configurations.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldNotOverwriteExistingConfiguration()
        {
            // Arrange
            var oldConfigPath = Path.Combine(this.testDirectory, "old-config.json");
            var oldConfigContent = "{\"Version\":\"1.0.0\"}";
            File.WriteAllText(oldConfigPath, oldConfigContent);

            var configName = "existing-config";
            var existingConfigContent = "{\"Version\":\"2.0.0\"}";
            
            // Create existing configuration
            var existingConfigPath = this.configurationManager.GetConfigurationPath(configName);
            var existingConfigDir = Path.GetDirectoryName(existingConfigPath);
            if (!Directory.Exists(existingConfigDir))
            {
                Directory.CreateDirectory(existingConfigDir);
            }
            File.WriteAllText(existingConfigPath, existingConfigContent);

            // Act
            var result = this.migrationService.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            Assert.IsFalse(result);
            
            // Verify existing configuration wasn't changed
            var currentContent = File.ReadAllText(existingConfigPath);
            Assert.AreEqual(existingConfigContent, currentContent);
        }

        /// <summary>
        /// Test migration with invalid JSON content.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldHandleInvalidJsonGracefully()
        {
            // Arrange
            var oldConfigPath = Path.Combine(this.testDirectory, "invalid-config.json");
            var invalidJsonContent = "{ invalid json content }";
            File.WriteAllText(oldConfigPath, invalidJsonContent);

            var configName = "invalid-json-config";

            // Act
            var result = this.migrationService.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            // Migration should still succeed (it just copies the file)
            Assert.IsTrue(result);
            Assert.IsTrue(this.configurationManager.ConfigurationExists(configName));
        }

        /// <summary>
        /// Test migration with read-only source file.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldHandleReadOnlySourceFile()
        {
            // Arrange
            var oldConfigPath = Path.Combine(this.testDirectory, "readonly-config.json");
            var configContent = "{\"Version\":\"1.0.0\"}";
            File.WriteAllText(oldConfigPath, configContent);

            // Make file read-only (if supported on platform)
            try
            {
                var fileInfo = new FileInfo(oldConfigPath);
                fileInfo.IsReadOnly = true;
            }
            catch
            {
                // Ignore if not supported on platform
            }

            var configName = "readonly-migrated-config";

            // Act
            var result = this.migrationService.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(this.configurationManager.ConfigurationExists(configName));
        }

        /// <summary>
        /// Test migration with very long file paths.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldHandleLongPaths()
        {
            // Arrange
            var longFileName = new string('a', 100) + ".json";
            var oldConfigPath = Path.Combine(this.testDirectory, longFileName);
            var configContent = "{\"Version\":\"1.0.0\"}";
            
            try
            {
                File.WriteAllText(oldConfigPath, configContent);
            }
            catch (PathTooLongException)
            {
                // Skip test if platform doesn't support long paths
                Assert.Inconclusive("Platform doesn't support long file paths");
                return;
            }

            var configName = "long-path-config";

            // Act
            var result = this.migrationService.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(this.configurationManager.ConfigurationExists(configName));
        }

        /// <summary>
        /// Test migration with empty configuration file.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldHandleEmptyFile()
        {
            // Arrange
            var oldConfigPath = Path.Combine(this.testDirectory, "empty-config.json");
            File.WriteAllText(oldConfigPath, string.Empty);

            var configName = "empty-migrated-config";

            // Act
            var result = this.migrationService.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(this.configurationManager.ConfigurationExists(configName));
        }
    }
}