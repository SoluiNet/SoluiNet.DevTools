// <copyright file="ConfigurationManagerTest.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.UnitTest
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoluiNet.DevTools.Core.Configuration;
    using SoluiNet.DevTools.Core.Configuration.Models;
    using SoluiNet.DevTools.Core.Services.Platform;

    /// <summary>
    /// Tests for the cross-platform configuration manager.
    /// </summary>
    [TestClass]
    public class ConfigurationManagerTest
    {
        private IPlatformService platformService;
        private CrossPlatformConfigurationManager configurationManager;
        private string testConfigDirectory;

        /// <summary>
        /// Initialize test setup.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.platformService = PlatformServiceFactory.Create();
            this.configurationManager = new CrossPlatformConfigurationManager(this.platformService);
            
            // Create a temporary test directory
            this.testConfigDirectory = Path.Combine(Path.GetTempPath(), "SoluiNetTest_" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(this.testConfigDirectory);
        }

        /// <summary>
        /// Clean up test resources.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(this.testConfigDirectory))
            {
                try
                {
                    Directory.Delete(this.testConfigDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        /// <summary>
        /// Test configuration manager creation with null platform service.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfigurationManager_Constructor_ShouldThrowOnNullPlatformService()
        {
            // Act
            _ = new CrossPlatformConfigurationManager(null);
        }

        /// <summary>
        /// Test getting configuration path.
        /// </summary>
        [TestMethod]
        public void GetConfigurationPath_ShouldReturnValidPath()
        {
            // Arrange
            var configName = "test-config";

            // Act
            var configPath = this.configurationManager.GetConfigurationPath(configName);

            // Assert
            Assert.IsNotNull(configPath);
            Assert.IsTrue(configPath.EndsWith(".json"));
            Assert.IsTrue(configPath.Contains(configName));
        }

        /// <summary>
        /// Test getting configuration path with null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetConfigurationPath_ShouldThrowOnNullName()
        {
            // Act
            this.configurationManager.GetConfigurationPath(null);
        }

        /// <summary>
        /// Test getting configuration path with empty name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetConfigurationPath_ShouldThrowOnEmptyName()
        {
            // Act
            this.configurationManager.GetConfigurationPath(string.Empty);
        }

        /// <summary>
        /// Test configuration exists for non-existent configuration.
        /// </summary>
        [TestMethod]
        public void ConfigurationExists_ShouldReturnFalseForNonExistent()
        {
            // Arrange
            var configName = "non-existent-config";

            // Act
            var exists = this.configurationManager.ConfigurationExists(configName);

            // Assert
            Assert.IsFalse(exists);
        }

        /// <summary>
        /// Test configuration exists with null name.
        /// </summary>
        [TestMethod]
        public void ConfigurationExists_ShouldReturnFalseForNull()
        {
            // Act
            var exists = this.configurationManager.ConfigurationExists(null);

            // Assert
            Assert.IsFalse(exists);
        }

        /// <summary>
        /// Test getting non-existent configuration.
        /// </summary>
        [TestMethod]
        public void GetConfiguration_ShouldReturnNullForNonExistent()
        {
            // Arrange
            var configName = "non-existent-config";

            // Act
            var config = this.configurationManager.GetConfiguration<ApplicationConfiguration>(configName);

            // Assert
            Assert.IsNull(config);
        }

        /// <summary>
        /// Test getting configuration with null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetConfiguration_ShouldThrowOnNullName()
        {
            // Act
            this.configurationManager.GetConfiguration<ApplicationConfiguration>(null);
        }

        /// <summary>
        /// Test saving configuration with null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveConfiguration_ShouldThrowOnNullName()
        {
            // Arrange
            var config = new ApplicationConfiguration();

            // Act
            this.configurationManager.SaveConfiguration(null, config);
        }

        /// <summary>
        /// Test saving null configuration.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveConfiguration_ShouldThrowOnNullConfiguration()
        {
            // Act
            this.configurationManager.SaveConfiguration<ApplicationConfiguration>("test", null);
        }

        /// <summary>
        /// Test configuration migration with non-existent source.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldReturnFalseForNonExistentSource()
        {
            // Arrange
            var oldPath = Path.Combine(this.testConfigDirectory, "non-existent.json");
            var configName = "test-config";

            // Act
            var result = this.configurationManager.MigrateConfiguration(oldPath, configName);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test configuration migration with null parameters.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldReturnFalseForNullParameters()
        {
            // Act & Assert
            Assert.IsFalse(this.configurationManager.MigrateConfiguration(null, "test"));
            Assert.IsFalse(this.configurationManager.MigrateConfiguration("test.json", null));
            Assert.IsFalse(this.configurationManager.MigrateConfiguration(null, null));
        }

        /// <summary>
        /// Test successful configuration migration.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldSucceedWithValidSource()
        {
            // Arrange
            var oldConfigPath = Path.Combine(this.testConfigDirectory, "old-config.json");
            var configContent = "{\"Version\":\"1.0.0\",\"EnabledPlugins\":{}}";
            File.WriteAllText(oldConfigPath, configContent);

            var configName = "migrated-config";

            // Act
            var result = this.configurationManager.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(this.configurationManager.ConfigurationExists(configName));
        }

        /// <summary>
        /// Test that migration doesn't overwrite existing configuration.
        /// </summary>
        [TestMethod]
        public void MigrateConfiguration_ShouldNotOverwriteExisting()
        {
            // Arrange
            var oldConfigPath = Path.Combine(this.testConfigDirectory, "old-config.json");
            var configContent = "{\"Version\":\"1.0.0\",\"EnabledPlugins\":{}}";
            File.WriteAllText(oldConfigPath, configContent);

            var configName = "existing-config";
            var existingConfig = new ApplicationConfiguration { Version = "2.0.0" };
            this.configurationManager.SaveConfiguration(configName, existingConfig);

            // Act
            var result = this.configurationManager.MigrateConfiguration(oldConfigPath, configName);

            // Assert
            Assert.IsFalse(result);
            
            // Verify existing config wasn't overwritten
            var loadedConfig = this.configurationManager.GetConfiguration<ApplicationConfiguration>(configName);
            Assert.IsNotNull(loadedConfig);
            Assert.AreEqual("2.0.0", loadedConfig.Version);
        }

        /// <summary>
        /// Test getting configuration directory.
        /// </summary>
        [TestMethod]
        public void GetConfigurationDirectory_ShouldReturnValidPath()
        {
            // Act
            var configDir = this.configurationManager.GetConfigurationDirectory();

            // Assert
            Assert.IsNotNull(configDir);
            Assert.IsTrue(configDir.Length > 0);
            Assert.IsTrue(Path.IsPathRooted(configDir));
        }

        /// <summary>
        /// Test complete save and load cycle.
        /// </summary>
        [TestMethod]
        public void SaveAndLoadConfiguration_ShouldWorkCorrectly()
        {
            // Arrange
            var configName = "test-save-load";
            var originalConfig = new ApplicationConfiguration
            {
                Version = "1.2.3",
                EnabledPlugins = new System.Collections.Generic.Dictionary<string, bool>
                {
                    { "Plugin1", true },
                    { "Plugin2", false }
                }
            };

            // Act
            this.configurationManager.SaveConfiguration(configName, originalConfig);
            var loadedConfig = this.configurationManager.GetConfiguration<ApplicationConfiguration>(configName);

            // Assert
            Assert.IsNotNull(loadedConfig);
            Assert.AreEqual(originalConfig.Version, loadedConfig.Version);
            Assert.AreEqual(originalConfig.EnabledPlugins.Count, loadedConfig.EnabledPlugins.Count);
            Assert.AreEqual(originalConfig.EnabledPlugins["Plugin1"], loadedConfig.EnabledPlugins["Plugin1"]);
            Assert.AreEqual(originalConfig.EnabledPlugins["Plugin2"], loadedConfig.EnabledPlugins["Plugin2"]);
            
            // Verify configuration exists
            Assert.IsTrue(this.configurationManager.ConfigurationExists(configName));
        }
    }
}